using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IMealDishService
    {
        // onlyOpen = true עבור status=open
        Task<List<MealDishDto>> GetByMealAsync(int mealId, bool onlyOpen, int currentUserId, CancellationToken ct = default);
        Task<MealDishDto> AddAsync(int mealId, CreateMealDishDto dto, int currentUserId, CancellationToken ct = default);
        Task RemoveAsync(int mealId, int mealDishId, int currentUserId, CancellationToken ct = default);
    }
}
