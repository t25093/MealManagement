using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Exceptions;
using MealManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Tests.Concurrency
{
    // Integration test מול PostgreSQL אמיתי: xmin קיימת רק שם
    public class MealDishConcurrencyTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _db;

        public MealDishConcurrencyTests(TestDatabaseFixture db)
        {
            _db = db;
        }

        // Seed: ארוחה חדשה עם מנה אחת. נדרשות 60, הוקצו 20, כלומר נותרו 40
        private async Task<int> SeedMealDishAsync()
        {
            await using var context = _db.CreateContext();

            var meal = new Meal
            {
                Name = "ארוחת בדיקה",
                MealType = MealType.Meat,
                EventDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                GuestsCount = 30,
                OwnerId = 1, // המנהלת מה-Seed
                MealDishes =
                {
                    new MealDish { Category = DishCategory.Side, RequiredQuantity = 60, AssignedQuantity = 20 }
                }
            };

            context.Meals.Add(meal);
            await context.SaveChangesAsync();

            return meal.MealDishes[0].Id;
        }

        [Fact]
        public async Task TwoContextsUpdateSameMealDish_SecondSaveThrowsConcurrencyException()
        {
            // 1. Arrange
            var mealDishId = await SeedMealDishAsync();

            // 2. שני מופעים נפרדים של AppDbContext, כמו שתי בקשות HTTP שונות
            await using var first = _db.CreateContext();
            await using var second = _db.CreateContext();

            // 3. שניהם קוראים את אותה שורה, עם אותו ערך xmin
            var dishA = await first.MealDishes.SingleAsync(md => md.Id == mealDishId);
            var dishB = await second.MealDishes.SingleAsync(md => md.Id == mealDishId);
            Assert.Equal(dishA.Version, dishB.Version);

            // 4. שניהם לוקחים את כל ה-40 שנותרו
            dishA.AssignedQuantity += 40;
            dishB.AssignedQuantity += 40;

            // 5. הראשון שומר בהצלחה
            await first.SaveChangesAsync();

            // 6. השני נכשל: ה-xmin שלו כבר לא תואם לשורה
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());

            // 7. בבסיס הנתונים נשמר רק העדכון של הראשון: 20 + 40 = 60, ולא 100
            await using var check = _db.CreateContext();
            var saved = await check.MealDishes.AsNoTracking().SingleAsync(md => md.Id == mealDishId);
            Assert.Equal(60, saved.AssignedQuantity);
        }

        [Fact]
        public async Task TwoContextsUpdateSameMealDish_ThroughUnitOfWork_ThrowsConflictException()
        {
            // Arrange
            var mealDishId = await SeedMealDishAsync();

            await using var first = _db.CreateContext();
            await using var second = _db.CreateContext();

            var dishA = await first.MealDishes.SingleAsync(md => md.Id == mealDishId);
            var dishB = await second.MealDishes.SingleAsync(md => md.Id == mealDishId);

            dishA.AssignedQuantity += 40;
            dishB.AssignedQuantity += 40;

            // Act: אותו תרחיש, אבל דרך ה-UnitOfWork האמיתי של הפרויקט
            await new UnitOfWork(first).SaveChangesAsync();

            // Assert: ה-UnitOfWork מתרגם את DbUpdateConcurrencyException ל-ConflictException, כלומר 409
            await Assert.ThrowsAsync<ConflictException>(() => new UnitOfWork(second).SaveChangesAsync());

            await using var check = _db.CreateContext();
            var saved = await check.MealDishes.AsNoTracking().SingleAsync(md => md.Id == mealDishId);
            Assert.Equal(60, saved.AssignedQuantity);
        }
    }
}
