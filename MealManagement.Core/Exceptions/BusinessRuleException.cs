namespace MealManagement.Core.Exceptions
{
    // 400: הפרה של כלל עסקי, למשל מאכל בשרי בארוחה חלבית
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
