using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly AppDbContext _context;

        public async Task<List<Dish>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        {
            var idList = ids.Distinct().ToList();

            return await _context.Dishes
                .AsNoTracking()
                .Where(d => idList.Contains(d.Id))
                .ToListAsync(ct);
        }

        public DishRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dish?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Dishes.FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<PagedResult<Dish>> GetPagedAsync(DishQueryParams query, CancellationToken ct = default)
        {
            var dishes = _context.Dishes.AsNoTracking();

            // 1. סינון
            if (!string.IsNullOrWhiteSpace(query.Search))
                dishes = dishes.Where(d => EF.Functions.ILike(d.Name, $"%{query.Search.Trim()}%"));

            if (query.Category.HasValue)
                dishes = dishes.Where(d => d.Category == query.Category.Value);

            if (query.KosherType.HasValue)
            {
                var kashrut = query.KosherType.Value;
                dishes = kashrut == MealType.Parve
                    ? dishes.Where(d => d.KosherType == MealType.Parve)
                    : dishes.Where(d => d.KosherType == kashrut || d.KosherType == MealType.Parve);
            }

            // 2. ספירה
            var totalCount = await dishes.CountAsync(ct);

            // 3. מיון
            dishes = query.SortBy?.ToLower() == "category"
                ? dishes.OrderBy(d => d.Category).ThenBy(d => d.Name)
                : dishes.OrderBy(d => d.Name);

            // 4. חיתוך, ורק אז שליפה
            var items = await dishes
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Dish>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> ExistsByNameAsync(string name, DishCategory category, CancellationToken ct = default)
        {
            return await _context.Dishes
                .AnyAsync(d => d.Category == category && EF.Functions.ILike(d.Name, name.Trim()), ct);
        }

        public async Task<bool> IsInUseAsync(int dishId, CancellationToken ct = default)
        {
            return await _context.MealDishes.AnyAsync(md => md.DishId == dishId, ct);
        }

        public async Task AddAsync(Dish dish, CancellationToken ct = default)
        {
            await _context.Dishes.AddAsync(dish, ct);
        }

        public void Remove(Dish dish)
        {
            _context.Dishes.Remove(dish);
        }
    }
}