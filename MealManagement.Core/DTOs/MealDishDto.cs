using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class MealDishDto
    {
        public int Id { get; set; }
        public int? DishId { get; set; }
        public string? DishName { get; set; }
        public DishCategory Category { get; set; }
        public MeasureUnit? Unit { get; set; }
        public double? RequiredQuantity { get; set; }
        public double AssignedQuantity { get; set; }
        public double? RemainingQuantity { get; set; }
        public bool IsFullyAssigned { get; set; }
        public List<AssignmentDto> Assignments { get; set; } = new();
    }
}