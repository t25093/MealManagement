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
    public class CreateMealDishDto
    {
        // ריק = מנה בלתי מוגדרת (רק קטגוריה)
        public int? DishId { get; set; }

        [EnumDataType(typeof(DishCategory))]
        public DishCategory Category { get; set; }
    }
}