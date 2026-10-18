using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "יש להזין אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string Email { get; set; } = string.Empty;
    }
}
