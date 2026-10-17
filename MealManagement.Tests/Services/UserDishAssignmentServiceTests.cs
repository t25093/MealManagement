using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Service.Services;
using MealManagement.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MealManagement.Tests.Services
{
    // בדיקות הלוגיקה של המשאב המוגבל: לקיחת מנה מתוך הכמות שנותרה
    public class UserDishAssignmentServiceTests
    {
        private const int MealId = 1;
        private const int MealDishId = 10;
        private const int OwnerId = 1;
        private const int ParticipantId = 2;

        private readonly Mock<IMealDishRepository> _mealDishes = new();
        private readonly Mock<IUserDishAssignmentRepository> _assignments = new();
        private readonly Mock<IMealRepository> _meals = new();
        private readonly Mock<IDishRepository> _dishes = new();
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private readonly UserDishAssignmentService _service;

        public UserDishAssignmentServiceTests()
        {
            // ברירת מחדל: המשתמשת היא משתתפת, ועוד לא לקחה מהמנה
            _meals.Setup(m => m.HasAccessAsync(MealId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            _users.Setup(u => u.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((int id, CancellationToken _) => new User { Id = id, FullName = $"משתמשת {id}" });

            _service = new UserDishAssignmentService(
                _mealDishes.Object,
                _assignments.Object,
                _meals.Object,
                _dishes.Object,
                _users.Object,
                _unitOfWork.Object,
                TestMapper.Create(),
                NullLogger<UserDishAssignmentService>.Instance);
        }

        // ===== פונקציות עזר =====

        private MealDish ArrangeMealDish(double required, double assigned, MealType mealType = MealType.Meat, int? dishId = 5)
        {
            var meal = new Meal { Id = MealId, MealType = mealType, GuestsCount = 30, OwnerId = OwnerId };

            var mealDish = new MealDish
            {
                Id = MealDishId,
                MealId = MealId,
                Meal = meal,
                DishId = dishId,
                Category = DishCategory.Main,
                RequiredQuantity = required,
                AssignedQuantity = assigned
            };

            _mealDishes.Setup(r => r.GetForUpdateAsync(MealDishId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(mealDish);

            return mealDish;
        }

        private static CreateAssignmentDto Take(double quantity) => new() { Quantity = quantity };

        // ===== ✅ מקרה תקין =====

        [Fact]
        public async Task TakeAsync_WhenEnoughRemaining_CreatesAssignmentAndSavesOnce()
        {
            // Arrange: נדרשות 60, הוקצו 20, כלומר נותרו 40
            var mealDish = ArrangeMealDish(required: 60, assigned: 20);

            // Act: מבקשים 15
            var result = await _service.TakeAsync(MealId, MealDishId, Take(15), ParticipantId);

            // Assert
            Assert.Equal(35, mealDish.AssignedQuantity);
            Assert.Equal(15, result.Quantity);
            _assignments.Verify(a => a.AddAsync(
                    It.Is<UserDishAssignment>(x => x.UserId == ParticipantId && x.Quantity == 15),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ===== ✅ פיצול =====

        [Fact]
        public async Task TakeAsync_TwoParticipantsSplitRemaining_BothSucceed()
        {
            // Arrange: נותרו 40
            var mealDish = ArrangeMealDish(required: 60, assigned: 20);

            const int participantA = 2;
            const int participantB = 3;

            // Act: כל אחת לוקחת 20
            await _service.TakeAsync(MealId, MealDishId, Take(20), participantA);
            await _service.TakeAsync(MealId, MealDishId, Take(20), participantB);

            // Assert: המנה מלאה, ונוצרו שתי הקצאות נפרדות
            Assert.Equal(60, mealDish.AssignedQuantity);
            _assignments.Verify(a => a.AddAsync(It.IsAny<UserDishAssignment>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        // ===== ❌ מקרה נדחה =====

        [Fact]
        public async Task TakeAsync_WhenNotEnoughRemaining_ThrowsConflictAndDoesNotSave()
        {
            // Arrange: נותרו 10
            var mealDish = ArrangeMealDish(required: 60, assigned: 50);

            // Act + Assert: מבקשים 15
            await Assert.ThrowsAsync<ConflictException>(() =>
                _service.TakeAsync(MealId, MealDishId, Take(15), ParticipantId));

            Assert.Equal(50, mealDish.AssignedQuantity);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // ===== ❌ התנגשות בשמירה =====

        [Fact]
        public async Task TakeAsync_WhenSaveHitsConcurrencyConflict_ExceptionIsNotSwallowed()
        {
            // Arrange: משתתפת אחרת עדכנה את השורה, וה-UnitOfWork זורק Conflict
            ArrangeMealDish(required: 60, assigned: 20);
            _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new ConflictException("המנה עודכנה על ידי משתתפת אחרת"));

            // Act + Assert: החריגה עוברת הלאה (ומשם ל-Middleware, שמחזיר 409)
            await Assert.ThrowsAsync<ConflictException>(() =>
                _service.TakeAsync(MealId, MealDishId, Take(15), ParticipantId));
        }

        // ===== ❌ הרשאה =====

        [Fact]
        public async Task TakeAsync_WhenUserIsNotParticipant_ThrowsForbidden()
        {
            // Arrange
            const int strangerId = 99;
            ArrangeMealDish(required: 60, assigned: 20);
            _meals.Setup(m => m.HasAccessAsync(MealId, strangerId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(false);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _service.TakeAsync(MealId, MealDishId, Take(5), strangerId));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // ===== מנה בלתי מוגדרת =====

        [Fact]
        public async Task TakeAsync_UndefinedDish_MeatDishInDairyMeal_ThrowsBusinessRule()
        {
            // Arrange: ארוחה חלבית, מנה עיקרית שעוד לא נבחר לה מאכל
            ArrangeMealDish(required: 0, assigned: 0, mealType: MealType.Dairy, dishId: null);

            var meatDish = new Dish { Id = 7, Name = "צלי בקר", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.2 };
            _dishes.Setup(d => d.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(meatDish);

            var dto = new CreateAssignmentDto { Quantity = 2, SpecifiedDishId = 7 };

            // Act + Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _service.TakeAsync(MealId, MealDishId, dto, ParticipantId));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TakeAsync_UndefinedDish_ParveDishInDairyMeal_SetsDishAndRequiredQuantity()
        {
            // Arrange: ארוחה חלבית עם 30 אורחים, ומאכל פרווה של 0.2 לאדם
            var mealDish = ArrangeMealDish(required: 0, assigned: 0, mealType: MealType.Dairy, dishId: null);

            var parveDish = new Dish { Id = 8, Name = "אורז", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 0.2 };
            _dishes.Setup(d => d.GetByIdAsync(8, It.IsAny<CancellationToken>())).ReturnsAsync(parveDish);

            var dto = new CreateAssignmentDto { Quantity = 2, SpecifiedDishId = 8 };

            // Act
            await _service.TakeAsync(MealId, MealDishId, dto, ParticipantId);

            // Assert: 0.2 × 30 = 6, ומתוכן נלקחו 2
            Assert.Same(parveDish, mealDish.Dish);
            Assert.Equal(6, mealDish.RequiredQuantity, precision: 3);
            Assert.Equal(2, mealDish.AssignedQuantity);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
