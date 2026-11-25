namespace MealManagement.Core.DTOs
{
    // משתתפת שכותבת עכשיו בצ'אט, והטקסט שכבר הקלידה (לפני השליחה)
    public class TypingStatusDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
