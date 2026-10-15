using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
        Task<UserDto> GetByIdAsync(int userId, CancellationToken ct = default);

        // Admin בלבד (נבדק ב-Controller)
        Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
