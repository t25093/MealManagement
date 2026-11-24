using MealManagement.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class CreateMealListItemDto
    {
        [Required(ErrorMessage = "יש לבחור רשימה")]
        public MealListType? ListType { get; set; }

        [Required(ErrorMessage = "יש להזין שם")]
        [StringLength(100, ErrorMessage = "השם ארוך מדי (עד 100 תווים)")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "ההערה ארוכה מדי (עד 200 תווים)")]
        public string? Note { get; set; }
    }
}
