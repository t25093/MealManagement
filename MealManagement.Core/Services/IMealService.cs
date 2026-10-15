using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IMealService
    {
        Task<MealDetailsDto> CreateAsync(CreateMealDto dto, int currentUserId, CancellationToken ct = default);
        Task<PagedResult<MealSummaryDto>> GetMyMealsAsync(MealQueryParams query, int currentUserId, CancellationToken ct = default);
        Task<MealDetailsDto> GetByIdAsync(int mealId, int currentUserId, CancellationToken ct = default);
        Task UpdateAsync(int mealId, UpdateMealDto dto, int currentUserId, CancellationToken ct = default);

        // בעלת הארוחה, או Admin
        Task DeleteAsync(int mealId, int currentUserId, bool isAdmin, CancellationToken ct = default);

        Task<UserDto> AddParticipantAsync(int mealId, AddParticipantDto dto, int currentUserId, CancellationToken ct = default);
        Task RemoveParticipantAsync(int mealId, int participantId, int currentUserId, CancellationToken ct = default);
    }
}
