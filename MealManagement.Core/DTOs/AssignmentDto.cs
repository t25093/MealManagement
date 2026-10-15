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
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
