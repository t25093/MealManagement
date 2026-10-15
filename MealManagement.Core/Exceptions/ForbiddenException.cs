namespace MealManagement.Core.Exceptions
{
    // 403: המשתמשת מחוברת, אבל אין לה הרשאה לארוחה הזאת
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
