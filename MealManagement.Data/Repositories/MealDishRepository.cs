using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class MealDishRepository : IMealDishRepository
    {
        private readonly AppDbContext _context;

        public MealDishRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MealDish?> GetForUpdateAsync(int mealDishId, CancellationToken ct = default)
        {
            // בלי AsNoTracking: ערך ה-xmin המקורי חייב להישמר במעקב
            return await _context.MealDishes
                .Include(md => md.Meal)
                .Include(md => md.Dish)
                .FirstOrDefaultAsync(md => md.Id == mealDishId, ct);
        }

        public async Task<List<MealDish>> GetByMealAsync(int mealId, bool onlyOpen, CancellationToken ct = default)
        {
            var query = _context.MealDishes
                .AsNoTracking()
                .Include(md => md.Dish)
                .Where(md => md.MealId == mealId);

            if (onlyOpen)
                query = query.Where(md => md.DishId == null || md.AssignedQuantity < md.RequiredQuantity);

            return await query
                .OrderBy(md => md.Category)
                .ThenBy(md => md.Id)
                .ToListAsync(ct);
        }

        public async Task<bool> HasAssignmentsAsync(int mealDishId, CancellationToken ct = default)
        {
            return await _context.UserDishAssignments.AnyAsync(a => a.MealDishId == mealDishId, ct);
        }

        public async Task AddAsync(MealDish mealDish, CancellationToken ct = default)
        {
            await _context.MealDishes.AddAsync(mealDish, ct);
        }

        public void Remove(MealDish mealDish)
        {
            _context.MealDishes.Remove(mealDish);
        }
    }
}