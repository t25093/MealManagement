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
    public class MealServiceTests
    {
        private const int OwnerId = 1;

        private readonly Mock<IMealRepository> _meals = new();
        private readonly Mock<IDishRepository> _dishes = new();
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUserDishAssignmentRepository> _assignments = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IEmailNotifications> _emails = new();

        private readonly MealService _service;

        public MealServiceTests()
        {
            _service = new MealService(
                _meals.Object,
                _dishes.Object,
                _users.Object,
                _assignments.Object,
                _unitOfWork.Object,
                _emails.Object,
                TestMapper.Create(),
                NullLogger<MealService>.Instance);
        }

        private void ArrangeDishes(params Dish[] dishes)
        {
            _dishes.Setup(d => d.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dishes.ToList());
        }

        // ===== חישוב כמויות =====

        [Fact]
        public async Task CreateAsync_CalculatesRequiredQuantity_PerPersonTimesGuests()
        {
            // Arrange: 0.2 ק"ג לאדם, 50 אורחים
            var kugel = new Dish { Id = 3, Name = "קוגל", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.2 };
            ArrangeDishes(kugel);

            // תופסים את הארוחה שה-Service מעביר ל-Repository
            Meal? saved = null;
            _meals.Setup(m => m.AddAsync(It.IsAny<Meal>(), It.IsAny<CancellationToken>()))
                  .Callback<Meal, CancellationToken>((meal, _) => saved = meal);
            _meals.Setup(m => m.GetDetailsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(() => saved);

            var dto = new CreateMealDto
            {
                Name = "שבת משפחתית",
                MealType = MealType.Meat,
                EventDate = DateTime.UtcNow.AddDays(7),
                GuestsCount = 50,
                MenuItems = { new CreateMealDishDto { DishId = 3, Category = DishCategory.Side } }
            };

            // Act
            await _service.CreateAsync(dto, OwnerId);

            // Assert: 0.2 × 50 = 10
            Assert.NotNull(saved);
            var mealDish = Assert.Single(saved!.MealDishes);
            Assert.Equal(10, mealDish.RequiredQuantity, precision: 3);
            Assert.Equal(0, mealDish.AssignedQuantity);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ===== ולידציית כשרות =====

        [Fact]
        public async Task CreateAsync_MeatDishInDairyMeal_ThrowsBusinessRuleAndDoesNotSave()
        {
            // Arrange
            var schnitzel = new Dish { Id = 4, Name = "שניצל", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 1 };
            ArrangeDishes(schnitzel);

            var dto = new CreateMealDto
            {
                Name = "ארוחת שבועות",
                MealType = MealType.Dairy,
                EventDate = DateTime.UtcNow.AddDays(7),
                GuestsCount = 20,
                MenuItems = { new CreateMealDishDto { DishId = 4, Category = DishCategory.Main } }
            };

            // Act + Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateAsync(dto, OwnerId));

            _meals.Verify(m => m.AddAsync(It.IsAny<Meal>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // ===== הורדת מספר אורחים =====

        [Fact]
        public async Task UpdateAsync_GuestsBelowAssignedQuantity_ThrowsConflictAndDoesNotSave()
        {
            // Arrange: 50 אורחים × 0.2 = 10 נדרשים, ו-8 כבר נלקחו
            var kugel = new Dish { Id = 3, Name = "קוגל", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.2 };
            ArrangeDishes(kugel);

            var meal = new Meal
            {
                Id = 1,
                OwnerId = OwnerId,
                GuestsCount = 50,
                MealDishes = { new MealDish { Id = 10, DishId = 3, RequiredQuantity = 10, AssignedQuantity = 8 } }
            };
            _meals.Setup(m => m.GetForUpdateAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(meal);

            // Act + Assert: 30 אורחים × 0.2 = 6, פחות מה-8 שכבר נלקחו
            await Assert.ThrowsAsync<ConflictException>(() =>
                _service.UpdateAsync(1, new UpdateMealDto { GuestsCount = 30 }, OwnerId));

            Assert.Equal(50, meal.GuestsCount);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // ===== הרשאה לפרטי ארוחה =====

        [Fact]
        public async Task GetByIdAsync_UserIsNotOwnerOrParticipant_ThrowsForbidden()
        {
            // Arrange: בעלת הארוחה 1, משתתפת 2
            var meal = new Meal
            {
                Id = 1,
                OwnerId = OwnerId,
                Participants = { new User { Id = 2 } }
            };
            _meals.Setup(m => m.GetDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(meal);

            // Act + Assert: משתמשת 99 מנסה לצפות
            await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetByIdAsync(1, 99));
        }

        // ===== הוספת משתתפת שלא רשומה =====

        [Fact]
        public async Task AddParticipantAsync_EmailNotRegistered_CreatesInvitation()
        {
            // Arrange: האימייל לא קיים במערכת
            var meal = new Meal { Id = 1, OwnerId = OwnerId };
            _meals.Setup(m => m.GetForUpdateAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(meal);
            _users.Setup(u => u.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync((User?)null);

            // Act (האימייל עובר נרמול לאותיות קטנות)
            var result = await _service.AddParticipantAsync(1, new AddParticipantDto { Email = " New@Test.com" }, OwnerId);

            // Assert: נשמרה הזמנה ולא נוספה משתתפת
            Assert.True(result.IsPending);
            var invitation = Assert.Single(meal.Invitations);
            Assert.Equal("new@test.com", invitation.Email);
            Assert.Empty(meal.Participants);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // ונשלח מייל הזמנה להרשמה
            _emails.Verify(e => e.SendMealInvitationAsync("new@test.com", meal, It.IsAny<string>(), false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddParticipantAsync_RegisteredUser_AddsParticipantAndSendsEmail()
        {
            // Arrange
            var meal = new Meal { Id = 1, OwnerId = OwnerId };
            var dina = new User { Id = 5, FullName = "דינה", Email = "dina@test.com" };
            _meals.Setup(m => m.GetForUpdateAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(meal);
            _users.Setup(u => u.GetByEmailAsync("dina@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(dina);
            _users.Setup(u => u.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(dina);
            _users.Setup(u => u.GetByIdAsync(OwnerId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new User { Id = OwnerId, FullName = "שרה" });

            // Act
            var result = await _service.AddParticipantAsync(1, new AddParticipantDto { Email = "dina@test.com" }, OwnerId);

            // Assert
            Assert.False(result.IsPending);
            Assert.Same(dina, Assert.Single(meal.Participants));
            _emails.Verify(e => e.SendMealInvitationAsync("dina@test.com", meal, "שרה", true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddParticipantAsync_EmailAlreadyInvited_ThrowsConflict()
        {
            // Arrange
            var meal = new Meal
            {
                Id = 1,
                OwnerId = OwnerId,
                Invitations = { new MealInvitation { MealId = 1, Email = "new@test.com" } }
            };
            _meals.Setup(m => m.GetForUpdateAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(meal);
            _users.Setup(u => u.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
                  .ReturnsAsync((User?)null);

            // Act + Assert
            await Assert.ThrowsAsync<ConflictException>(() =>
                _service.AddParticipantAsync(1, new AddParticipantDto { Email = "new@test.com" }, OwnerId));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _emails.Verify(e => e.SendMealInvitationAsync(It.IsAny<string>(), It.IsAny<Meal>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ===== הרשאת עריכה =====

        private const int ParticipantId = 2;

        private Meal ArrangeMealWithParticipant(bool participantCanEdit)
        {
            var participant = new User { Id = ParticipantId };
            var meal = new Meal { Id = 1, OwnerId = OwnerId, Name = "ארוחה", Participants = { participant } };
            if (participantCanEdit)
                meal.Editors.Add(participant);

            _meals.Setup(m => m.GetForUpdateAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(meal);
            return meal;
        }

        [Fact]
        public async Task UpdateAsync_ParticipantWithEditPermission_UpdatesMeal()
        {
            // Arrange
            var meal = ArrangeMealWithParticipant(participantCanEdit: true);

            // Act
            await _service.UpdateAsync(1, new UpdateMealDto { Name = "ארוחת שבת" }, ParticipantId);

            // Assert
            Assert.Equal("ארוחת שבת", meal.Name);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ParticipantWithoutEditPermission_ThrowsForbiddenAndDoesNotSave()
        {
            // Arrange
            var meal = ArrangeMealWithParticipant(participantCanEdit: false);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _service.UpdateAsync(1, new UpdateMealDto { Name = "ארוחת שבת" }, ParticipantId));

            Assert.Equal("ארוחה", meal.Name);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SetEditPermissionAsync_OwnerGrantsAndRevokes_UpdatesEditors()
        {
            // Arrange
            var meal = ArrangeMealWithParticipant(participantCanEdit: false);

            // Act + Assert: מתן הרשאה
            await _service.SetEditPermissionAsync(1, ParticipantId, canEdit: true, OwnerId);
            Assert.Equal(ParticipantId, Assert.Single(meal.Editors).Id);

            // Act + Assert: ביטול ההרשאה
            await _service.SetEditPermissionAsync(1, ParticipantId, canEdit: false, OwnerId);
            Assert.Empty(meal.Editors);

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task SetEditPermissionAsync_ByEditorNotOwner_ThrowsForbidden()
        {
            // Arrange: גם עורכת לא יכולה לתת הרשאות, רק בעלת הארוחה
            ArrangeMealWithParticipant(participantCanEdit: true);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _service.SetEditPermissionAsync(1, ParticipantId, canEdit: false, ParticipantId));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SetEditPermissionAsync_UserIsNotParticipant_ThrowsNotFound()
        {
            // Arrange
            ArrangeMealWithParticipant(participantCanEdit: false);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.SetEditPermissionAsync(1, 99, canEdit: true, OwnerId));
        }

        [Fact]
        public async Task RemoveParticipantAsync_Editor_AlsoLosesEditPermission()
        {
            // Arrange
            var meal = ArrangeMealWithParticipant(participantCanEdit: true);
            _assignments.Setup(a => a.GetByMealAsync(1, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserDishAssignment>());

            // Act
            await _service.RemoveParticipantAsync(1, ParticipantId, OwnerId);

            // Assert
            Assert.Empty(meal.Participants);
            Assert.Empty(meal.Editors);
        }
    }
}
