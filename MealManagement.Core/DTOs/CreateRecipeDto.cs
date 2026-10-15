using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class CreateRecipeDto
    {
        // אופציונלי: למאכל מסוים בארוחה
        public int? DishId { get; set; }

        [Required(ErrorMessage = "יש להזין כותרת")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין את תוכן המתכון")]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;
    }
}