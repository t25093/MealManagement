using MealManagement.Core.Entities;

namespace MealManagement.Service.Services
{
    // מי רשאית לערוך ארוחה: בעלת הארוחה, או משתתפת שקיבלה ממנה הרשאת עריכה
    public static class MealPermissions
    {
        public static bool CanEdit(Meal meal, int userId)
        {
            return meal.OwnerId == userId || meal.Editors.Any(e => e.Id == userId);
        }
    }
}
