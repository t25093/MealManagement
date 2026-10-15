using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    // MealDishId לא כאן: הוא מגיע מהכתובת
    // POST /api/meals/{mealId}/dishes/{mealDishId}/assignments
    public class CreateAssignmentDto
    {
        [Range(typeof(decimal), "0.01", "100000", ErrorMessage = "הכמות חייבת להיות גדולה מ-0")]
        public decimal Quantity { get; set; }

        // למנה בלתי מוגדרת בלבד: בחירת מאכל קיים מהמאגר
        public int? SpecifiedDishId { get; set; }

        // למנה בלתי מוגדרת בלבד: הוספת מאכל חדש למאגר
        public CreateDishDto? NewDish { get; set; }
    }
}
