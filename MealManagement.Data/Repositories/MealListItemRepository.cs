using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class MealListItemRepository : IMealListItemRepository
    {
        private readonly AppDbContext _context;

        public MealListItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MealListItem?> GetByIdAsync(int itemId, CancellationToken ct = default)
        {
            return await _context.MealListItems
                .Include(i => i.DoneBy)
                .Include(i => i.CreatedBy)
                .FirstOrDefaultAsync(i => i.Id == itemId, ct);
        }

        public async Task<List<MealListItem>> GetByMealAsync(int mealId, CancellationToken ct = default)
        {
            return await _context.MealListItems
                .AsNoTracking()
                .Include(i => i.DoneBy)
                .Include(i => i.CreatedBy)
                .Where(i => i.MealId == mealId)
                .OrderBy(i => i.CreatedAt)
                .ThenBy(i => i.Id)
                .ToListAsync(ct);
        }

        public async Task AddAsync(MealListItem item, CancellationToken ct = default)
        {
            await _context.MealListItems.AddAsync(item, ct);
        }

        public void Remove(MealListItem item)
        {
            _context.MealListItems.Remove(item);
        }
    }
}
