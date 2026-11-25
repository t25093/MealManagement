namespace MealManagement.Core.DTOs
{
    // הגדרות אישיות של המשתמשת המחוברת
    public class UpdatePreferencesDto
    {
        // האם המשתתפות רואות את הטקסט שלה בצ'אט עוד לפני השליחה
        public bool ShowTypingText { get; set; } = true;
    }
}
