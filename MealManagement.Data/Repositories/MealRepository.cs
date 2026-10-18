using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class MealRepository : IMealRepository
    {
        private readonly AppDbContext _context;

        public MealRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Meal?> GetDetailsAsync(int mealId, CancellationToken ct = default)
        {
            return await _context.Meals
                .AsNoTracking()
                .Include(m => m.Owner)
                .Include(m => m.Participants)
                .Include(m => m.Invitations)
                .Include(m => m.MealDishes)
                    .ThenInclude(md => md.Dish)
                .Include(m => m.MealDishes)
                    .ThenInclude(md => md.Assignments)
                        .ThenInclude(a => a.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == mealId, ct);
        }

        public async Task<Meal?> GetForUpdateAsync(int mealId, CancellationToken ct = default)
        {
            return await _context.Meals
                .Include(m => m.MealDishes)
                .Include(m => m.Participants)
                .Include(m => m.Invitations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == mealId, ct);
        }

        public async Task<PagedResult<Meal>> GetPagedForUserAsync(int userId, MealQueryParams query, CancellationToken ct = default)
        {
            var meals = _context.Meals.AsNoTracking();

            // 1. סינון לפי תפקיד
            var role = query.Role?.Trim().ToLower();
            meals = role switch
            {
                "owner" => meals.Where(m => m.OwnerId == userId),
                "participant" => meals.Where(m => m.Participants.Any(p => p.Id == userId)),
                _ => meals.Where(m => m.OwnerId == userId || m.Participants.Any(p => p.Id == userId))
            };

            // 2. חיפוש לפי שם
            if (!string.IsNullOrWhiteSpace(query.Search))
                meals = meals.Where(m => EF.Functions.ILike(m.Name, $"%{query.Search.Trim()}%"));

            // 3. ספירה
            var totalCount = await meals.CountAsync(ct);

            // 4. מיון
            meals = query.SortBy?.ToLower() == "name"
                ? meals.OrderBy(m => m.Name).ThenBy(m => m.Id)
                : meals.OrderBy(m => m.EventDate).ThenBy(m => m.Id);

            // 5. חיתוך ושליפה
            var items = await meals
                .Include(m => m.Owner)
                .Include(m => m.MealDishes)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Meal>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> HasAccessAsync(int mealId, int userId, CancellationToken ct = default)
        {
            return await _context.Meals.AnyAsync(m =>
                m.Id == mealId &&
                (m.OwnerId == userId || m.Participants.Any(p => p.Id == userId)), ct);
        }

        public async Task<bool> ExistsAsync(int mealId, CancellationToken ct = default)
        {
            return await _context.Meals.AnyAsync(m => m.Id == mealId, ct);
        }

        public async Task AddAsync(Meal meal, CancellationToken ct = default)
        {
            await _context.Meals.AddAsync(meal, ct);
        }

        public void Remove(Meal meal)
        {
            _context.Meals.Remove(meal);
        }
    }
}