using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class MealDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MealType MealType { get; set; }
        public DateTime EventDate { get; set; }
        public int GuestsCount { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
        public List<UserDto> Participants { get; set; } = new();
        public List<string> PendingEmails { get; set; } = new(); // הוזמנו ועדיין לא נרשמו
        public List<MealDishDto> MenuItems { get; set; } = new();
    }
}
