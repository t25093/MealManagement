using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    // עריכת הפרופיל: שם מלא ואימייל. שינוי אימייל דורש את הסיסמה הנוכחית.
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "יש להזין שם מלא")]
        [StringLength(100, ErrorMessage = "השם יכול להכיל עד 100 תווים")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        // חובה רק כשהאימייל משתנה
        public string? CurrentPassword { get; set; }
    }
}
