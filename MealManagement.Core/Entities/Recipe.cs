using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public Meal Meal { get; set; }
        public string Title { get; set; }
        public int? DishId { get; set; }
        public Dish? Dish { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; }
    }
}
