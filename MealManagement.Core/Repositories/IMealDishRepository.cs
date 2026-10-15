using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IMealDishRepository
    {
        // ⭐ עם מעקב (בלי AsNoTracking!) כדי שערך ה-xmin המקורי יישמר
        Task<MealDish?> GetForUpdateAsync(int mealDishId, CancellationToken ct = default);

        // AsNoTracking. onlyOpen = רק מנות שנותרה בהן כמות
        Task<List<MealDish>> GetByMealAsync(int mealId, bool onlyOpen, CancellationToken ct = default);

        Task<bool> HasAssignmentsAsync(int mealDishId, CancellationToken ct = default);

        Task AddAsync(MealDish mealDish, CancellationToken ct = default);
        void Remove(MealDish mealDish);
    }
}
