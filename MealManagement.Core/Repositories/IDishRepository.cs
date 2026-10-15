using MealManagement.Core.Enums;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IDishRepository
    {
        // עם מעקב: לעדכון ולמחיקה
        Task<Dish?> GetByIdAsync(int id, CancellationToken ct = default);

        // AsNoTracking: סינון, מיון ו-Skip/Take בתוך השאילתה
        Task<PagedResult<Dish>> GetPagedAsync(DishQueryParams query, CancellationToken ct = default);

        // לבדיקת שם כפול באותה קטגוריה
        Task<bool> ExistsByNameAsync(string name, DishCategory category, CancellationToken ct = default);

        // מאכל שנמצא בשימוש בארוחה לא נמחק
        Task<bool> IsInUseAsync(int dishId, CancellationToken ct = default);

        Task AddAsync(Dish dish, CancellationToken ct = default);
        void Remove(Dish dish);
    }
}
