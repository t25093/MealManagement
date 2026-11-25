using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    // מי כותבת עכשיו בצ'אט של כל ארוחה. נשמר בזיכרון בלבד (לא בבסיס הנתונים),
    // ורשומה שלא התעדכנה כמה שניות נחשבת כאילו הפסיקה לכתוב.
    public interface IChatTypingStore
    {
        void Set(int mealId, int userId, string userName, string text);
        void Clear(int mealId, int userId);

        // מי שכותבת עכשיו בארוחה, בלי exceptUserId (את עצמי לא מציגים)
        List<TypingStatusDto> GetActive(int mealId, int exceptUserId);
    }
}
