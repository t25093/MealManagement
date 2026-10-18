using MealManagement.Core.Entities;
using MealManagement.Core.Services;
using MealManagement.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;

namespace MealManagement.Service.Email
{
    public class EmailNotifications : IEmailNotifications
    {
        private readonly IEmailSender _sender;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailNotifications> _logger;

        public EmailNotifications(IEmailSender sender, IOptions<EmailSettings> settings, ILogger<EmailNotifications> logger)
        {
            _sender = sender;
            _settings = settings.Value;
            _logger = logger;
        }

        public Task SendMealInvitationAsync(string toEmail, Meal meal, string inviterName, bool isRegistered, CancellationToken ct = default)
        {
            var mealName = WebUtility.HtmlEncode(meal.Name);
            var inviter = WebUtility.HtmlEncode(inviterName);
            var when = FormatIsraelTime(meal.EventDate);

            string subject, body;
            if (isRegistered)
            {
                subject = $"צורפת לארוחה \"{meal.Name}\"";
                body = $"<p>{inviter} צירפה אותך לארוחה <b>{mealName}</b> ב-{when}.</p>"
                     + $"<p>אפשר להיכנס לארוחה, לבחור מה להכין ולהצטרף לצ'אט:</p>"
                     + Button(Link($"/meals/{meal.Id}"), "לארוחה");
            }
            else
            {
                subject = $"הוזמנת לארוחה \"{meal.Name}\"";
                body = $"<p>{inviter} הזמינה אותך לארוחה <b>{mealName}</b> ב-{when}.</p>"
                     + $"<p>כדי להצטרף, נרשמים לאתר עם כתובת המייל הזו ({WebUtility.HtmlEncode(toEmail)}), והארוחה תופיע מיד ברשימה \"ארוחות ששותפתי בהן\".</p>"
                     + Button(Link("/login"), "להרשמה");
            }

            return SendSafelyAsync(toEmail, subject, Layout(body), ct);
        }

        public Task SendPasswordResetAsync(User user, string token, CancellationToken ct = default)
        {
            var body = $"<p>שלום {WebUtility.HtmlEncode(user.FullName)},</p>"
                     + "<p>התקבלה בקשה לאפס את הסיסמה שלך. לבחירת סיסמה חדשה:</p>"
                     + Button(Link($"/reset-password?token={Uri.EscapeDataString(token)}"), "בחירת סיסמה חדשה")
                     + "<p>הקישור תקף לשעה אחת. אם לא ביקשת לאפס את הסיסמה, אפשר להתעלם מהמייל.</p>";

            return SendSafelyAsync(user.Email, "איפוס סיסמה למנה-לי", Layout(body), ct);
        }

        // מייל שלא נשלח לא מבטל את הפעולה (המשתתפת כבר נוספה), ולכן השגיאה רק נרשמת ביומן
        private async Task SendSafelyAsync(string toEmail, string subject, string html, CancellationToken ct)
        {
            try
            {
                await _sender.SendAsync(toEmail, subject, html, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email \"{Subject}\"", subject);
            }
        }

        private string Link(string path)
        {
            return _settings.ClientUrl.TrimEnd('/') + path;
        }

        private static string Button(string url, string text)
        {
            return $"<p><a href=\"{WebUtility.HtmlEncode(url)}\" style=\"display:inline-block;padding:10px 22px;background:#1d5c4d;color:#ffffff;border-radius:8px;text-decoration:none;font-weight:bold\">{text}</a></p>";
        }

        private static string Layout(string body)
        {
            return "<div dir=\"rtl\" style=\"font-family:Arial,sans-serif;font-size:15px;line-height:1.6;color:#1f2a24;max-width:520px\">"
                 + "<h2 style=\"color:#1d5c4d;margin:0 0 12px\">מנה-לי</h2>"
                 + body
                 + "<p style=\"color:#888;font-size:12px;margin-top:24px\">כל אחת מביאה מנה, כולן נהנות</p>"
                 + "</div>";
        }

        // התאריך נשמר ב-UTC, ובמייל מוצג בשעון ישראל
        private static string FormatIsraelTime(DateTime utc)
        {
            var local = TimeZoneInfo.TryFindSystemTimeZoneById("Asia/Jerusalem", out var israel)
                ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), israel)
                : utc;
            return local.ToString("dd/MM/yyyy 'בשעה' HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
