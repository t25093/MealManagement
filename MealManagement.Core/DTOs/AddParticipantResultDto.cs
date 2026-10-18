namespace MealManagement.Core.DTOs
{
    // תוצאת הוספת משתתפת: נוספה מיד (רשומה), או נשמרה הזמנה (עדיין לא רשומה)
    public class AddParticipantResultDto
    {
        public string Email { get; set; } = string.Empty;
        public bool IsPending { get; set; }
        public UserDto? User { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
