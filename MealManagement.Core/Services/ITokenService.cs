using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    // יצירת JWT. נקרא מתוך UserService בהרשמה ובהתחברות
    public interface ITokenService
    {
        AuthResponseDto CreateToken(UserDto user);
    }
}
