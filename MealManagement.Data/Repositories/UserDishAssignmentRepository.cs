using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class UserDishAssignmentRepository : IUserDishAssignmentRepository
    {
        private readonly AppDbContext _context;

        public UserDishAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserDishAssignment?> GetForUpdateAsync(int assignmentId, CancellationToken ct = default)
        {
            return await _context.UserDishAssignments
                .Include(a => a.MealDish)
                    .ThenInclude(md => md.Meal)
                .FirstOrDefaultAsync(a => a.Id == assignmentId, ct);
        }

        public async Task<UserDishAssignment?> GetByMealDishAndUserAsync(int mealDishId, int userId, CancellationToken ct = default)
        {
            return await _context.UserDishAssignments
                .FirstOrDefaultAsync(a => a.MealDishId == mealDishId && a.UserId == userId, ct);
        }

        public async Task<List<UserDishAssignment>> GetByMealAsync(int mealId, CancellationToken ct = default)
        {
            return await _context.UserDishAssignments
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.MealDish)
                    .ThenInclude(md => md.Dish)
                .Where(a => a.MealDish.MealId == mealId)
                .OrderBy(a => a.MealDishId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(UserDishAssignment assignment, CancellationToken ct = default)
        {
            await _context.UserDishAssignments.AddAsync(assignment, ct);
        }

        public void Remove(UserDishAssignment assignment)
        {
            _context.UserDishAssignments.Remove(assignment);
        }
    }
}