using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
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
        private readonly Mock<IUserDishAssignmentRepository> _assignments = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<ITokenService> _tokenService = new();
        private readonly Mock<IEmailNotifications> _emails = new();

        private readonly UserService _service;

        public UserServiceTests()
        {
            // ברירת מחדל: אין שיבוצים שמחכים לאימייל
            _assignments.Setup(a => a.GetPendingByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserDishAssignment>());

            _service = new UserService(
                _users.Object,
                _invitations.Object,
                _assignments.Object,
                _unitOfWork.Object,
                _passwordHasher.Object,
                _tokenService.Object,
                _emails.Object,
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
        public async Task LoginAsync_EmailWithSpaces_FindsUser()
        {
            // Arrange: רווח באמצע ובסוף, למשל מהמקלדת בטלפון
            var user = new User { Id = 5, Email = "dina@test.com", PasswordHash = "hash" };
            _users.Setup(u => u.GetByEmailAsync("dina@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);
            _passwordHasher.Setup(h => h.Verify("123456", "hash")).Returns(true);
            _tokenService.Setup(t => t.CreateToken(It.IsAny<UserDto>())).Returns(new AuthResponseDto { Token = "token" });

            var dto = new LoginDto { Email = "Dina @test.com ", Password = "123456" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.Equal("token", result.Token);
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

        [Fact]
        public async Task DeleteAsync_OtherUserWithoutAdmin_ThrowsForbiddenAndDoesNotSave()
        {
            // Act + Assert: משתמשת 5 מנסה למחוק את משתמשת 6
            await Assert.ThrowsAsync<ForbiddenException>(() => _service.DeleteAsync(6, 5, isAdmin: false));

            _users.Verify(u => u.RemoveWithRelatedDataAsync(It.IsAny<User>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_AdminAccount_ThrowsConflictAndDoesNotSave()
        {
            // Arrange
            var admin = new User { Id = 1, Role = UserRole.Admin };
            _users.Setup(u => u.GetForDeleteAsync(1, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(admin);

            // Act + Assert
            await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteAsync(1, 1, isAdmin: true));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_OwnAccount_ReturnsTakenQuantityAndRemovesUser()
        {
            // Arrange: המשתמשת לקחה 3 מתוך 5 שהוקצו במנה
            var mealDish = new MealDish { Id = 9, RequiredQuantity = 10, AssignedQuantity = 5 };
            var user = new User
            {
                Id = 5,
                Role = UserRole.User,
                Assignments = { new UserDishAssignment { UserId = 5, MealDish = mealDish, Quantity = 3 } }
            };
            _users.Setup(u => u.GetForDeleteAsync(5, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);
            _users.Setup(u => u.GetAnyAdminIdAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(1);

            // Act
            await _service.DeleteAsync(5, 5, isAdmin: false);

            // Assert: הכמות חזרה למנה, המאכלים עוברים למנהלת 1, ושמירה אחת
            Assert.Equal(2, mealDish.AssignedQuantity);
            _users.Verify(u => u.RemoveWithRelatedDataAsync(user, 1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ===== איפוס סיסמה =====

        private static string Sha256Hex(string value)
        {
            return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value)));
        }

        [Fact]
        public async Task ForgotPasswordAsync_UnknownEmail_DoesNothingAndDoesNotReveal()
        {
            // Arrange
            _users.Setup(u => u.GetByEmailAsync("nobody@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync((User?)null);

            // Act: לא נזרקת שגיאה
            await _service.ForgotPasswordAsync(new ForgotPasswordDto { Email = "nobody@test.com" });

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _emails.Verify(e => e.SendPasswordResetAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPasswordAsync_KnownEmail_SavesTokenHashAndSendsEmail()
        {
            // Arrange
            var user = new User { Id = 5, FullName = "דינה", Email = "dina@test.com" };
            _users.Setup(u => u.GetByEmailAsync("dina@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _users.Setup(u => u.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            string? sentToken = null;
            _emails.Setup(e => e.SendPasswordResetAsync(user, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                   .Callback<User, string, CancellationToken>((_, token, _) => sentToken = token);

            // Act
            await _service.ForgotPasswordAsync(new ForgotPasswordDto { Email = "Dina@Test.com" });

            // Assert: במייל נשלח הקוד, ונשמר רק ה-hash שלו, לשעה אחת
            Assert.NotNull(sentToken);
            Assert.Equal(Sha256Hex(sentToken!), user.PasswordResetTokenHash);
            Assert.InRange(user.PasswordResetExpiresAt!.Value, DateTime.UtcNow.AddMinutes(59), DateTime.UtcNow.AddMinutes(61));
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_ValidToken_ChangesPasswordAndClearsToken()
        {
            // Arrange
            var user = new User
            {
                Id = 5,
                PasswordHash = "old",
                PasswordResetTokenHash = Sha256Hex("abc"),
                PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };
            _users.Setup(u => u.GetByPasswordResetTokenHashAsync(Sha256Hex("abc"), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);
            _passwordHasher.Setup(h => h.Hash("newpass")).Returns("new-hash");

            // Act
            await _service.ResetPasswordAsync(new ResetPasswordDto { Token = "abc", NewPassword = "newpass" });

            // Assert: הסיסמה הוחלפה, והקוד לא יעבוד שוב
            Assert.Equal("new-hash", user.PasswordHash);
            Assert.Null(user.PasswordResetTokenHash);
            Assert.Null(user.PasswordResetExpiresAt);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_ExpiredToken_ThrowsBusinessRuleAndDoesNotSave()
        {
            // Arrange
            var user = new User
            {
                Id = 5,
                PasswordHash = "old",
                PasswordResetTokenHash = Sha256Hex("abc"),
                PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(-1)
            };
            _users.Setup(u => u.GetByPasswordResetTokenHashAsync(Sha256Hex("abc"), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(user);

            // Act + Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _service.ResetPasswordAsync(new ResetPasswordDto { Token = "abc", NewPassword = "newpass" }));

            Assert.Equal("old", user.PasswordHash);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_PendingAssignments_MoveToTheNewUser()
        {
            var pending = new UserDishAssignment { PendingEmail = "guest@test.com", Quantity = 2 };
            _assignments.Setup(a => a.GetPendingByEmailAsync("guest@test.com", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserDishAssignment> { pending });
            _invitations.Setup(i => i.GetByEmailAsync("guest@test.com", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<MealInvitation>());
            _tokenService.Setup(t => t.CreateToken(It.IsAny<UserDto>())).Returns(new AuthResponseDto());

            await _service.RegisterAsync(new RegisterDto { FullName = "אורחת", Email = "Guest@test.com", Password = "123456" });

            Assert.NotNull(pending.User);
            Assert.Equal("guest@test.com", pending.User!.Email);
            Assert.Null(pending.PendingEmail);
        }

        // ===== עריכת פרופיל =====

        private User ExistingUser() => new()
        {
            Id = 5,
            FullName = "דינה",
            Email = "dina@test.com",
            PasswordHash = "hash",
            Role = UserRole.User,
            PasswordResetTokenHash = "reset",
            PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        private void SetupProfileUser(User user)
        {
            _users.Setup(u => u.GetWithParticipationsAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _tokenService.Setup(t => t.CreateToken(It.IsAny<UserDto>()))
                         .Returns((UserDto dto) => new AuthResponseDto { Token = "new-token", User = dto });
            _invitations.Setup(i => i.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<MealInvitation>());
        }

        [Fact]
        public async Task UpdateProfileAsync_NameOnly_SavesWithoutPasswordAndReturnsNewToken()
        {
            var user = ExistingUser();
            SetupProfileUser(user);

            // אותו אימייל באותיות גדולות וברווחים: לא נחשב שינוי, ולכן לא צריך סיסמה
            var result = await _service.UpdateProfileAsync(5, new UpdateProfileDto { FullName = "  דינה כהן ", Email = " Dina@Test.com" });

            Assert.Equal("דינה כהן", user.FullName);
            Assert.Equal("dina@test.com", user.Email);
            Assert.Equal("new-token", result.Token);
            Assert.Equal("דינה כהן", result.User.FullName);
            Assert.Equal("reset", user.PasswordResetTokenHash); // האימייל לא השתנה: קישור האיפוס נשאר
            _passwordHasher.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("wrong")]
        public async Task UpdateProfileAsync_EmailChangedWithoutCorrectPassword_ThrowsAndDoesNotSave(string? password)
        {
            var user = ExistingUser();
            SetupProfileUser(user);
            _passwordHasher.Setup(h => h.Verify("wrong", "hash")).Returns(false);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.UpdateProfileAsync(5,
                new UpdateProfileDto { FullName = "דינה", Email = "new@test.com", CurrentPassword = password }));

            Assert.Equal("dina@test.com", user.Email);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProfileAsync_EmailTaken_ThrowsConflictAndDoesNotSave()
        {
            var user = ExistingUser();
            SetupProfileUser(user);
            _passwordHasher.Setup(h => h.Verify("123456", "hash")).Returns(true);
            _users.Setup(u => u.EmailExistsAsync("rachel@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => _service.UpdateProfileAsync(5,
                new UpdateProfileDto { FullName = "דינה", Email = "Rachel@Test.com", CurrentPassword = "123456" }));

            Assert.Equal("dina@test.com", user.Email);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProfileAsync_EmailChanged_SavesNormalizedEmailClearsResetLinkAndJoinsInvitedMeals()
        {
            var user = ExistingUser();
            var alreadyIn = new Meal { Id = 1, OwnerId = 9 };
            user.ParticipatingMeals.Add(alreadyIn);
            SetupProfileUser(user);
            _passwordHasher.Setup(h => h.Verify("123456", "hash")).Returns(true);

            var newMeal = new Meal { Id = 2, OwnerId = 9 };
            var invitations = new List<MealInvitation>
            {
                new() { MealId = 1, Meal = alreadyIn, Email = "new@test.com" },
                new() { MealId = 2, Meal = newMeal, Email = "new@test.com" }
            };
            _invitations.Setup(i => i.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(invitations);

            await _service.UpdateProfileAsync(5,
                new UpdateProfileDto { FullName = "דינה", Email = " New@Test.com ", CurrentPassword = "123456" });

            Assert.Equal("new@test.com", user.Email);
            Assert.Null(user.PasswordResetTokenHash);
            Assert.Null(user.PasswordResetExpiresAt);
            // הארוחה החדשה נוספה, וארוחה שכבר משתתפת בה לא נוספה פעמיים
            Assert.Equal(new[] { 1, 2 }, user.ParticipatingMeals.Select(m => m.Id).OrderBy(id => id));
            _invitations.Verify(i => i.RemoveRange(invitations), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_EmailChanged_PendingAssignmentsMoveOrMergeIntoExisting()
        {
            var user = ExistingUser();
            SetupProfileUser(user);
            _passwordHasher.Setup(h => h.Verify("123456", "hash")).Returns(true);

            var pendingNew = new UserDishAssignment { MealDishId = 10, PendingEmail = "new@test.com", Quantity = 2 };
            var pendingSameDish = new UserDishAssignment { MealDishId = 11, PendingEmail = "new@test.com", Quantity = 3 };
            var existing = new UserDishAssignment { MealDishId = 11, UserId = 5, Quantity = 1 };
            _assignments.Setup(a => a.GetPendingByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserDishAssignment> { pendingNew, pendingSameDish });
            _assignments.Setup(a => a.GetByMealDishAndUserAsync(11, 5, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

            await _service.UpdateProfileAsync(5,
                new UpdateProfileDto { FullName = "דינה", Email = "new@test.com", CurrentPassword = "123456" });

            Assert.Equal(5, pendingNew.UserId);
            Assert.Null(pendingNew.PendingEmail);
            Assert.Equal(4, existing.Quantity);
            _assignments.Verify(a => a.Remove(pendingSameDish), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_UnknownUser_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateProfileAsync(77, new UpdateProfileDto { FullName = "דינה", Email = "dina@test.com" }));
        }

        [Fact]
        public async Task UpdatePreferencesAsync_HideTypingText_SavesAndReturnsIt()
        {
            var user = ExistingUser();
            _users.Setup(u => u.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            var result = await _service.UpdatePreferencesAsync(5, new UpdatePreferencesDto { ShowTypingText = false });

            Assert.False(user.ShowTypingText);
            Assert.False(result.ShowTypingText);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePreferencesAsync_UnknownUser_ThrowsNotFoundAndDoesNotSave()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdatePreferencesAsync(77, new UpdatePreferencesDto { ShowTypingText = false }));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
