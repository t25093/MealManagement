using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // null: מוזמנת שעוד לא נרשמה
        public string UserName { get; set; } = string.Empty; // למוזמנת שלא נרשמה: האימייל שלה
        public bool IsPending { get; set; }
        public double Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
