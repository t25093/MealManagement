namespace MealManagement.Core.Exceptions
{
    // 401: פרטי התחברות שגויים
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}