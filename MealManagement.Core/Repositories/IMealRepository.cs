using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IMealRepository
    {
        // AsNoTracking, עם כל הפרטים לתצוגה (Include של מנות, מאכלים, הקצאות ומשתתפות)
        Task<Meal?> GetDetailsAsync(int mealId, CancellationToken ct = default);

        // עם מעקב: לעדכון, למחיקה ולהוספת משתתפות (כולל MealDishes ו-Participants)
        Task<Meal?> GetForUpdateAsync(int mealId, CancellationToken ct = default);

        // role: "owner", "participant" או ריק לשניהם
        Task<PagedResult<Meal>> GetPagedForUserAsync(int userId, MealQueryParams query, CancellationToken ct = default);

        // האם המשתמשת היא בעלת הארוחה או משתתפת בה
        Task<bool> HasAccessAsync(int mealId, int userId, CancellationToken ct = default);

        Task<bool> ExistsAsync(int mealId, CancellationToken ct = default);

        Task AddAsync(Meal meal, CancellationToken ct = default);
        void Remove(Meal meal);
    }
}
