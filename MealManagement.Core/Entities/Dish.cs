using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MealManagement.Core.Entities
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DishCategory Category { get; set; } // קטגוריית המאכל (Enum)
        public double QuantityPerPerson { get; set; }
        public MeasureUnit Unit { get; set; }
        public MealType KosherType { get; set; }
        public int CreatedByUserId { get; set; }
        public List<MealDish> MealDishes { get; set; } = new();
    }
}
