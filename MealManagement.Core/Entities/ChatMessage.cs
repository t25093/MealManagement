using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int MealId { get; set; }
        public Meal Meal { get; set; }
    }
}
