using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class UserDishAssignment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int MealDishId { get; set; }
        public MealDish MealDish { get; set; }
        public DateTime CreatedAt { get; set; } // כמות שהמשתמשת לוקחת להכין
        public double Quantity { get; set; } // עדכון מאכל מותאם אישית במידת הצורך
    }
}
