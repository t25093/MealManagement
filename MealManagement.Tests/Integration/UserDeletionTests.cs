using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Services;
using MealManagement.Data;
using MealManagement.Data.Repositories;
using MealManagement.Service.Services;
using MealManagement.Tests.Concurrency;
using MealManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MealManagement.Tests.Integration
{
    // Integration test מול PostgreSQL אמיתי: בודק שהמחיקה עוברת את כל המפתחות הזרים בשמירה אחת
    [Collection("PostgreSQL")]
    public class UserDeletionTests : IClassFixture<TestDatabaseFixture>
    {
        private const int AdminId = 1; // המנהלת מה-Seed

        private readonly TestDatabaseFixture _db;

        public UserDeletionTests(TestDatabaseFixture db)
        {
            _db = db;
        }

        private static UserService CreateService(AppDbContext context)
        {
            return new UserService(
                new UserRepository(context),
                new MealInvitationRepository(context),
                new UnitOfWork(context),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<ITokenService>(),
                Mock.Of<IEmailNotifications>(),
                TestMapper.Create(),
                NullLogger<UserService>.Instance);
        }

        private static User NewUser(string name)
        {
            return new User
            {
                FullName = name,
                Email = $"{Guid.NewGuid():N}@test.com",
                PasswordHash = "hash",
                Role = UserRole.User,
                CreateAt = DateTime.UtcNow
            };
        }

        private static Meal NewMeal(User owner, string name)
        {
            return new Meal
            {
                Name = name,
                MealType = MealType.Parve,
                EventDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                GuestsCount = 10,
                Owner = owner
            };
        }

        [Fact]
        public async Task DeleteAsync_UserWithMealsAndActivity_RemovesHerDataAndKeepsOthers()
        {
            // Arrange: דינה (שתימחק) ורחל. לכל אחת ארוחה, ודינה פעילה גם בארוחה של רחל
            int dinaId, dinaMealId, rachelMealId, rachelMealDishId, dinaDishId;
            await using (var seed = _db.CreateContext())
            {
                var dina = NewUser("דינה");
                var rachel = NewUser("רחל");
                seed.Users.AddRange(dina, rachel);
                await seed.SaveChangesAsync();

                // מאכל שדינה הוסיפה למאגר, ורחל משתמשת בו בארוחה שלה
                var dinaDish = new Dish
                {
                    Name = "עוגה של דינה",
                    Category = DishCategory.Dessert,
                    KosherType = MealType.Parve,
                    QuantityPerPerson = 1,
                    Unit = MeasureUnit.Portion,
                    CreatedByUserId = dina.Id
                };
                seed.Dishes.Add(dinaDish);

                // הארוחה של דינה: רחל משתתפת, לקחה מנה וכתבה הודעה
                var dinaMeal = NewMeal(dina, "הארוחה של דינה");
                var dinaMealDish = new MealDish { Category = DishCategory.Side, RequiredQuantity = 10, AssignedQuantity = 4 };
                dinaMeal.MealDishes.Add(dinaMealDish);
                dinaMeal.Participants.Add(rachel);
                dinaMeal.Invitations.Add(new MealInvitation { Email = "guest@test.com", CreatedAt = DateTime.UtcNow });
                dinaMeal.Messages.Add(new ChatMessage { Content = "שלום", SentAt = DateTime.UtcNow, Sender = rachel });
                dinaMealDish.Assignments.Add(new UserDishAssignment { User = rachel, Quantity = 4, CreatedAt = DateTime.UtcNow });

                // הארוחה של רחל: דינה משתתפת, לקחה 3 מתוך 5, כתבה הודעה ומתכון
                var rachelMeal = NewMeal(rachel, "הארוחה של רחל");
                var rachelMealDish = new MealDish { Category = DishCategory.Dessert, Dish = dinaDish, RequiredQuantity = 10, AssignedQuantity = 5 };
                rachelMeal.MealDishes.Add(rachelMealDish);
                rachelMeal.Participants.Add(dina);
                rachelMeal.Messages.Add(new ChatMessage { Content = "אני מביאה עוגה", SentAt = DateTime.UtcNow, Sender = dina });
                rachelMeal.Messages.Add(new ChatMessage { Content = "תודה", SentAt = DateTime.UtcNow, Sender = rachel });
                rachelMeal.Recipes.Add(new Recipe { Title = "עוגה", Content = "מתכון", CreatedAt = DateTime.UtcNow, Author = dina, Dish = dinaDish });
                rachelMealDish.Assignments.Add(new UserDishAssignment { User = dina, Quantity = 3, CreatedAt = DateTime.UtcNow });
                rachelMealDish.Assignments.Add(new UserDishAssignment { User = rachel, Quantity = 2, CreatedAt = DateTime.UtcNow });

                seed.Meals.AddRange(dinaMeal, rachelMeal);
                await seed.SaveChangesAsync();

                dinaId = dina.Id;
                dinaMealId = dinaMeal.Id;
                rachelMealId = rachelMeal.Id;
                rachelMealDishId = rachelMealDish.Id;
                dinaDishId = dinaDish.Id;
            }

            // Act: דינה מוחקת את החשבון שלה
            await using (var context = _db.CreateContext())
            {
                await CreateService(context).DeleteAsync(dinaId, dinaId, isAdmin: false);
            }

            // Assert
            await using var check = _db.CreateContext();

            Assert.False(await check.Users.AnyAsync(u => u.Id == dinaId));
            Assert.False(await check.Meals.AnyAsync(m => m.Id == dinaMealId));
            Assert.False(await check.MealInvitations.AnyAsync(i => i.MealId == dinaMealId));

            // הארוחה של רחל נשארה, בלי הפעילות של דינה
            var rachelMealAfter = await check.Meals
                .Include(m => m.Participants)
                .Include(m => m.Messages)
                .Include(m => m.Recipes)
                .SingleAsync(m => m.Id == rachelMealId);
            Assert.Empty(rachelMealAfter.Participants);
            Assert.Equal("תודה", Assert.Single(rachelMealAfter.Messages).Content);
            Assert.Empty(rachelMealAfter.Recipes);

            // ה-3 שדינה לקחה חזרו להיות פנויים, והלקיחה של רחל נשארה
            var mealDish = await check.MealDishes.Include(md => md.Assignments).SingleAsync(md => md.Id == rachelMealDishId);
            Assert.Equal(2, mealDish.AssignedQuantity);
            Assert.Equal(2, Assert.Single(mealDish.Assignments).Quantity);

            // המאכל שדינה הוסיפה נשאר במאגר ועבר למנהלת
            var dish = await check.Dishes.SingleAsync(d => d.Id == dinaDishId);
            Assert.Equal(AdminId, dish.CreatedByUserId);
        }
    }
}
