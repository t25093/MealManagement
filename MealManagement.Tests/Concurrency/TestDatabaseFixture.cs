using MealManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MealManagement.Tests.Concurrency
{
    // בסיס נתונים ייעודי לבדיקות: נוצר פעם אחת לפני הבדיקות ונמחק בסופן
    public class TestDatabaseFixture : IAsyncLifetime
    {
        private readonly string _connectionString;

        public TestDatabaseFixture()
        {
            // ה-connection string לא נכתב בקוד: User Secrets של פרויקט הבדיקות, או משתנה סביבה
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<TestDatabaseFixture>()
                .AddEnvironmentVariables()
                .Build();

            _connectionString = configuration.GetConnectionString("TestDb")
                ?? throw new InvalidOperationException(
                    "חסר ConnectionStrings:TestDb. יש להגדיר אותו ב-User Secrets של MealManagement.Tests.");
        }

        // כל קריאה מחזירה מופע חדש ונפרד של AppDbContext
        public AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_connectionString)
                .Options;

            return new AppDbContext(options);
        }

        // Setup: בסיס נקי, עם כל ה-migrations וה-Seed שבהן
        public async Task InitializeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }

        // Teardown: מחיקת בסיס הנתונים של הבדיקות
        public async Task DisposeAsync()
        {
            await using var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
        }
    }
}
