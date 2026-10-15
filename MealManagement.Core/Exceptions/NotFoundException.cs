namespace MealManagement.Core.Exceptions
{
    // 404: הפריט המבוקש לא קיים (ארוחה, מנה, מאכל, משתמשת)
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
