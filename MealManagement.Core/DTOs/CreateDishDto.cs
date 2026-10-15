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
    public class CreateDishDto
    {
        [Required(ErrorMessage = "יש להזין שם מאכל")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [EnumDataType(typeof(DishCategory))]
        public DishCategory Category { get; set; }

        [EnumDataType(typeof(MealType))]
        public MealType KosherType { get; set; }

        [Range(typeof(double), "0.01", "10000", ErrorMessage = "הכמות לאדם חייבת להיות גדולה מ-0")]
        public double QuantityPerPerson { get; set; }

        [EnumDataType(typeof(MeasureUnit))]
        public MeasureUnit Unit { get; set; }
    }
}