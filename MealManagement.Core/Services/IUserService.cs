using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
        Task<UserDto> GetByIdAsync(int userId, CancellationToken ct = default);

        // עריכת שם ואימייל של המשתמשת המחוברת. מחזיר token חדש עם הפרטים המעודכנים
        Task<AuthResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken ct = default);

        // הגדרות אישיות (כרגע: האם רואים את הטקסט שלה בצ'אט בזמן הקלדה)
        Task<UserDto> UpdatePreferencesAsync(int userId, UpdatePreferencesDto dto, CancellationToken ct = default);

        // שולח מייל עם קישור לאיפוס סיסמה. לא מגלה אם האימייל רשום במערכת
        Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default);
        Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default);

        // Admin בלבד (נבדק ב-Controller)
        Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);

        // משתמשת מוחקת את החשבון של עצמה, ו-Admin יכולה למחוק כל משתמשת שאינה Admin
        Task DeleteAsync(int userId, int currentUserId, bool isAdmin, CancellationToken ct = default);
    }
}
