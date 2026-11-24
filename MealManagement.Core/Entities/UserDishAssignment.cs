using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class UserDishAssignment
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // null: שובצה מוזמנת שעוד לא נרשמה
        public User? User { get; set; }

        // מוזמנת שעוד לא נרשמה: האימייל שלה. כשהיא נרשמת, השיבוץ עובר אליה (UserId) והשדה מתרוקן
        public string? PendingEmail { get; set; }
        public int MealDishId { get; set; }
        public MealDish MealDish { get; set; }
        public DateTime CreatedAt { get; set; } // כמות שהמשתמשת לוקחת להכין
        public double Quantity { get; set; } // עדכון מאכל מותאם אישית במידת הצורך
    }
}
