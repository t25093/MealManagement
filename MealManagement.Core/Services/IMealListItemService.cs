using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    // רשימת הקניות ורשימת הדברים להביא. כל מי שיש לה גישה לארוחה יכולה להוסיף, לסמן, לערוך ולמחוק.
    public interface IMealListItemService
    {
        Task<List<MealListItemDto>> GetByMealAsync(int mealId, int currentUserId, CancellationToken ct = default);
        Task<MealListItemDto> CreateAsync(int mealId, CreateMealListItemDto dto, int currentUserId, CancellationToken ct = default);
        Task<MealListItemDto> UpdateAsync(int mealId, int itemId, UpdateMealListItemDto dto, int currentUserId, CancellationToken ct = default);
        Task DeleteAsync(int mealId, int itemId, int currentUserId, CancellationToken ct = default);
    }
}
