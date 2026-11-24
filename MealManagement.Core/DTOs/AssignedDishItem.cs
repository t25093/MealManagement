using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    // שורה אחת במייל למשובצת: מנה, כמות ויחידה
    public record AssignedDishItem(string DishName, double Quantity, MeasureUnit Unit);
}
