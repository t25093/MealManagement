namespace MealManagement.Core.Exceptions
{
    // 409: התנגשות. למשל אימייל כפול, כמות שלא נותרה, או מנה שעודכנה במקביל
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
