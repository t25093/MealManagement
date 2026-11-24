using System.ComponentModel.DataAnnotations;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    // PATCH: שדה ריק = לא משנים אותו
    public class UpdateMealDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [Range(1, 1000, ErrorMessage = "מספר האורחים חייב להיות בין 1 ל-1000")]
        public int? GuestsCount { get; set; }

        public DateTime? EventDate { get; set; }

        // סוג הארוחה (חלבית / בשרית)
        [EnumDataType(typeof(MealType), ErrorMessage = "סוג ארוחה לא תקין")]
        public MealType? MealType { get; set; }
    }
}
