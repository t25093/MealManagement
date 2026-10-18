namespace MealManagement.Core.Entities
{
    // הזמנה לארוחה לאימייל שעדיין לא רשום במערכת.
    // כשמשתמשת נרשמת עם האימייל הזה, ההזמנה הופכת להשתתפות ונמחקת.
    public class MealInvitation
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Meal Meal { get; set; } = null!;
    }
}
