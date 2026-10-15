using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class DishDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DishCategory Category { get; set; }
        public MealType KosherType { get; set; }
        public double QuantityPerPerson { get; set; }
        public MeasureUnit Unit { get; set; }
    }
}