using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    // עדכון חלקי: רק השדות שנשלחו משתנים
    public class UpdateMealListItemDto
    {
        [StringLength(100, ErrorMessage = "השם ארוך מדי (עד 100 תווים)")]
        public string? Name { get; set; }

        // נשלח כשעורכים את ההערה. מחרוזת ריקה מוחקת אותה.
        [StringLength(200, ErrorMessage = "ההערה ארוכה מדי (עד 200 תווים)")]
        public string? Note { get; set; }

        // סימון "נקנה" / "הובא"
        public bool? IsDone { get; set; }
    }
}
