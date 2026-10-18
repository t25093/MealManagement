using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using MealManagement.Service.Services;
using MealManagement.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MealManagement.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IMealInvitationRepository> _invitations = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<ITokenService> _tokenService = new();

        private readonly UserService _service;

        public UserServiceTests()
        {
            _service = new UserService(
                _users.Object,
                _invitations.Object,
                _unitOfWork.Object,
                _passwordHasher.Object,
                _tokenService.Object,
                TestMapper.Create(),
                NullLogger<UserService>.Instance);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ThrowsConflictAndDoesNotSave()
        {
            // Arrange
            _users.Setup(u => u.EmailExistsAsync("dina@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            var dto = new RegisterDto { FullName = "דינה", Email = "Dina@Test.com ", Password = "123456" };

            // Act + Assert (האימייל עובר נרמול לאותיות קטנות לפני הבדיקה)
            await Assert.ThrowsAsync<ConflictException>(() => _service.RegisterAsync(dto));

            _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedAndDoesNotCreateToken()
        {
            // Arrange
            var user = new User { Id = 5, Email = "dina@test.com", PasswordHash = "hash" };
            _users.Setup(u => u.GetByEmailAsync("dina@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);
            _passwordHasher.Setup(h => h.Verify("wrong", "hash")).Returns(false);

            var dto = new LoginDto { Email = "dina@test.com", Password = "wrong" };

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(dto));

            _tokenService.Verify(t => t.CreateToken(It.IsAny<UserDto>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_EmailWasInvited_JoinsMealsAndRemovesInvitations()
        {
            // Arrange: האימייל הוזמן לארוחה 7 לפני שנרשם
            var meal = new Meal { Id = 7 };
            var invitations = new List<MealInvitation>
            {
                new MealInvitation { MealId = 7, Email = "dina@test.com", Meal = meal }
            };
            _users.Setup(u => u.EmailExistsAsync("dina@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(false);
            _invitations.Setup(i => i.GetByEmailAsync("dina@test.com", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(invitations);
            _passwordHasher.Setup(h => h.Hash("123456")).Returns("hash");

            User? added = null;
            _users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                  .Callback<User, CancellationToken>((user, _) => added = user);

            var dto = new RegisterDto { FullName = "דינה", Email = "Dina@Test.com", Password = "123456" };

            // Act
            await _service.RegisterAsync(dto);

            // Assert: המשתמשת צורפה לארוחה, וההזמנה נמחקה, בשמירה אחת
            Assert.NotNull(added);
            Assert.Same(meal, Assert.Single(added!.ParticipatingMeals));
            _invitations.Verify(i => i.RemoveRange(invitations), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
