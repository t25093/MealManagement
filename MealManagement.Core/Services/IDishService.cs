using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IDishService
    {
        Task<PagedResult<DishDto>> GetAllAsync(DishQueryParams query, CancellationToken ct = default);
        Task<DishDto> GetByIdAsync(int dishId, CancellationToken ct = default);
        Task<DishDto> CreateAsync(CreateDishDto dto, int currentUserId, CancellationToken ct = default);

        // Admin בלבד (נבדק ב-Controller)
        Task UpdateAsync(int dishId, CreateDishDto dto, CancellationToken ct = default);
        Task DeleteAsync(int dishId, CancellationToken ct = default);
    }
}
