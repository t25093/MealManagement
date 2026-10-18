using MealManagement.Core.Entities;

namespace MealManagement.Core.Services
{
    // המיילים שהמערכת שולחת. כישלון בשליחה נרשם ביומן ולא מפיל את הפעולה עצמה
    public interface IEmailNotifications
    {
        // isRegistered: משתמשת רשומה מקבלת קישור לארוחה, ומי שלא רשומה מקבלת הזמנה להירשם
        Task SendMealInvitationAsync(string toEmail, Meal meal, string inviterName, bool isRegistered, CancellationToken ct = default);

        Task SendPasswordResetAsync(User user, string token, CancellationToken ct = default);
    }
}
