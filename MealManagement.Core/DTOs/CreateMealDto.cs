using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class CreateMealDto
    {
        [Required(ErrorMessage = "יש להזין שם ארוחה")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [EnumDataType(typeof(MealType))]
        public MealType MealType { get; set; }

        [Required(ErrorMessage = "יש להזין תאריך ארוחה")]
        public DateTime EventDate { get; set; }

        [Range(1, 1000, ErrorMessage = "מספר האורחים חייב להיות בין 1 ל-1000")]
        public int GuestsCount { get; set; }

        public List<CreateMealDishDto> MenuItems { get; set; } = new();
    }
}
