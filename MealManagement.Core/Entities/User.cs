using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreateAt { get; set; }

        // איפוס סיסמה: נשמר רק ה-hash של הקוד שנשלח במייל, ועד מתי הוא תקף
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetExpiresAt { get; set; }
        public List<Meal> OwnedMeals { get; set; } = new();
        public List<Meal> ParticipatingMeals { get; set; } = new();
        public List<Meal> EditableMeals { get; set; } = new(); // ארוחות שקיבלה בהן הרשאת עריכה
        public List<UserDishAssignment> Assignments { get; set; } = new();
        public List<ChatMessage> Messages { get; set; } = new();
    }
}
