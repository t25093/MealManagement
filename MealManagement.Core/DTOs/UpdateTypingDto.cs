using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    // הטקסט שבשדה ההודעה כרגע. ריק = הפסיקה לכתוב (מחקה או שלחה)
    public class UpdateTypingDto
    {
        [StringLength(1000, ErrorMessage = "ההודעה ארוכה מדי")]
        public string? Text { get; set; }

        // משתמשת שמסתירה את הטקסט שולחת רק IsTyping, בלי Text.
        // בלי IsTyping: כותבת אם יש טקסט
        public bool? IsTyping { get; set; }
    }
}
