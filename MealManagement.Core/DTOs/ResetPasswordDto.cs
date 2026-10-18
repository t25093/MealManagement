using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class ResetPasswordDto
    {
        // הקוד מהקישור שנשלח במייל
        [Required(ErrorMessage = "חסר קוד איפוס")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין סיסמה")]
        [MinLength(6, ErrorMessage = "הסיסמה חייבת להכיל לפחות 6 תווים")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
