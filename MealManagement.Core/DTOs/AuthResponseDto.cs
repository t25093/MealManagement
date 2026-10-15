namespace MealManagement.Core.DTOs
{
    // מה שהשרת מחזיר אחרי הרשמה או התחברות מוצלחת
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();
    }
}