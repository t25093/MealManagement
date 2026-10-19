using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using MealManagement.Service.Services;
using MealManagement.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MealManagement.Tests.Services
{
    public class MealDishServiceTests
    {
        private const int OwnerId = 1;

        private readonly Mock<IMealDishRepository> _mealDishes = new();
        private readonly Mock<IMealRepository> _meals = new();
        private readonly Mock<IDishRepository> _dishes = new();
        private readonly Mock<IUserDishAssignmentRepository> _assignments = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private readonly MealDishService _service;

        public MealDishServiceTests()
        {
            _service = new MealDishService(
                _mealDishes.Object,
                _meals.Object,
                _dishes.Object,
                _assignments.Object,
                _unitOfWork.Object,
                TestMapper.Create(),
                NullLogger<MealDishService>.Instance);
        }

        [Fact]
        public async Task AddAsync_DishAlreadyInMenu_AddsAnotherRow()
        {
            // Arrange: קוגל כבר בתפריט, ומוסיפים אותו שוב
            var kugel = new Dish { Id = 3, Name = "קוגל", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.2 };
            var meal = new Meal { Id = 7, OwnerId = OwnerId, MealType = MealType.Meat, GuestsCount = 50 };
            meal.MealDishes.Add(new MealDish { Id = 1, MealId = 7, DishId = 3, Dish = kugel, Category = DishCategory.Side, RequiredQuantity = 10 });

            _meals.Setup(m => m.GetForUpdateAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(meal);
            _dishes.Setup(d => d.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(kugel);

            MealDish? added = null;
            _mealDishes.Setup(m => m.AddAsync(It.IsAny<MealDish>(), It.IsAny<CancellationToken>()))
                       .Callback<MealDish, CancellationToken>((md, _) => added = md);

            // Act
            await _service.AddAsync(7, new CreateMealDishDto { DishId = 3 }, OwnerId);

            // Assert: שורה חדשה עם הכמות המלאה, והשמירה בוצעה
            Assert.NotNull(added);
            Assert.Equal(3, added!.DishId);
            Assert.Equal(10, added.RequiredQuantity, 3);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
