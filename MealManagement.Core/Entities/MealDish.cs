using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class MealDish
    {
        public int Id { get; set; }

        public int MealId { get; set; }
        public Meal Meal { get; set; }
        public int? DishId { get; set; }
        public Dish Dish { get; set; }
        public double RequiredQuantity { get; set; }
        public double AssignedQuantity { get; set; }
        public uint Version { get; set; }
        public DishCategory Category { get; set; }
        public List<UserDishAssignment> Assignments { get; set; } = new();
    }
}
