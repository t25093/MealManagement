using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IMealListItemRepository
    {
        Task<MealListItem?> GetByIdAsync(int itemId, CancellationToken ct = default);

        // AsNoTracking, כולל שמות מי שהוסיפה ומי שסימנה
        Task<List<MealListItem>> GetByMealAsync(int mealId, CancellationToken ct = default);

        Task AddAsync(MealListItem item, CancellationToken ct = default);
        void Remove(MealListItem item);
    }
}
