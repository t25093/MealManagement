using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data
{
    public class AppDbContext : DbContext
    {
        // ההגדרות (כולל ה-connection string) מגיעות מבחוץ, מ-Program.cs.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Meal> Meals => Set<Meal>();
        public DbSet<Dish> Dishes => Set<Dish>();
        public DbSet<MealDish> MealDishes => Set<MealDish>();
        public DbSet<UserDishAssignment> UserDishAssignments => Set<UserDishAssignment>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<MealInvitation> MealInvitations => Set<MealInvitation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // טוען את כל קבצי ההגדרות מתיקיית Configurations (שלב 4.2)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            
        }
    }
}