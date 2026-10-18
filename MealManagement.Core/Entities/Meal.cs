using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.Entities
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MealType MealType { get; set; }
        public DateTime EventDate { get; set; }
        public int GuestsCount { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User Owner { get; set; }
        public List<MealDish> MealDishes { get; set; } = new(); // רשימת המאכלים המקושרים לארוחה זו
        public List<ChatMessage> Messages { get; set; } = new();
        public List<User> Participants { get; set; } = new();
        public List<Recipe> Recipes { get; set; } = new();
        public List<MealInvitation> Invitations { get; set; } = new(); // אימיילים שהוזמנו ועדיין לא נרשמו
    }
}
