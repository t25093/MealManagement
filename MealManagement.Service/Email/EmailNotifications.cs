using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Services;
using MealManagement.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Text;

namespace MealManagement.Service.Email
{
    // המיילים שהמערכת שולחת. העיצוב תואם לאתר: פס עליון בגוון כחול-סגול, כרטיס לבן וכפתור בצבע האתר.
    // תוכנות מייל (Gmail, Outlook) מתעלמות מ-CSS חיצוני, ולכן הכול בנוי מטבלאות עם style בתוך כל תגית.
    public class EmailNotifications : IEmailNotifications
    {
        // צבעי האתר
        private const string Primary = "#5b5fef";
        private const string HeaderBg = "#312e81";
        private const string HeaderGradient = "linear-gradient(135deg,#1e1b4b 0%,#312e81 50%,#134e4a 100%)";
        private const string PageBg = "#f4f5fa";
        private const string Text = "#171b2c";
        private const string Muted = "#6d7590";
        private const string Border = "#e3e6f0";
        private const string Soft = "#f6f7fb";
        private const string Font = "'Rubik','Segoe UI',Arial,Helvetica,sans-serif";

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

            var details = DetailsTable(
                ("ארוחה", $"<b>{mealName}</b>"),
                ("מתי", when),
                ("סוג", MealTypeName(meal.MealType)),
                ("אורחים", meal.GuestsCount > 0 ? meal.GuestsCount.ToString(CultureInfo.InvariantCulture) : null),
                ("הזמינה אותך", inviter));

            string subject, preheader, body;
            if (isRegistered)
            {
                var url = Link($"/meals/{meal.Id}");
                subject = $"צורפת לארוחה \"{meal.Name}\"";
                preheader = $"{inviterName} צירפה אותך לארוחה. אפשר כבר לבחור מה להביא.";
                body = Heading("צורפת לארוחה חדשה")
                     + Paragraph($"{inviter} צירפה אותך לארוחה <b>{mealName}</b> ב-{when}.")
                     + details
                     + Paragraph("אפשר להיכנס לארוחה, לבחור מה להכין ולהצטרף לצ'אט של המשתתפות.")
                     + Button(url, "מעבר לארוחה")
                     + FallbackLink(url);
            }
            else
            {
                var url = Link("/login");
                subject = $"הוזמנת לארוחה \"{meal.Name}\"";
                preheader = $"{inviterName} הזמינה אותך לארוחה. נרשמים ומצטרפים בדקה.";
                body = Heading("הוזמנת לארוחה")
                     + Paragraph($"{inviter} הזמינה אותך לארוחה <b>{mealName}</b> ב-{when}.")
                     + details
                     + Paragraph($"כדי להצטרף, נרשמים לאתר עם כתובת המייל הזו (<span dir=\"ltr\">{WebUtility.HtmlEncode(toEmail)}</span>), והארוחה תופיע מיד ברשימה \"ארוחות ששותפתי בהן\".")
                     + Button(url, "להרשמה ולהצטרפות")
                     + FallbackLink(url);
            }

            return SendSafelyAsync(toEmail, subject, Layout(preheader, body), ct);
        }

        public Task SendPasswordResetAsync(User user, string token, CancellationToken ct = default)
        {
            var url = Link($"/reset-password?token={Uri.EscapeDataString(token)}");
            var body = Heading("איפוס סיסמה")
                     + Paragraph($"שלום {WebUtility.HtmlEncode(user.FullName)},")
                     + Paragraph("התקבלה בקשה לאפס את הסיסמה שלך במנה-לי. לבחירת סיסמה חדשה:")
                     + Button(url, "בחירת סיסמה חדשה")
                     + Note("הקישור תקף לשעה אחת. אם לא ביקשת לאפס את הסיסמה, אפשר להתעלם מהמייל, והסיסמה הנוכחית תישאר בתוקף.")
                     + FallbackLink(url);

            return SendSafelyAsync(user.Email, "איפוס סיסמה למנה-לי", Layout("קישור לבחירת סיסמה חדשה, תקף לשעה אחת.", body), ct);
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

        // ===== אבני בניין של המייל =====

        private static string Heading(string text)
        {
            return $"<h1 style=\"margin:0 0 14px;font-family:{Font};font-size:24px;line-height:1.3;font-weight:800;color:{Text}\">{text}</h1>";
        }

        private static string Paragraph(string html)
        {
            return $"<p style=\"margin:0 0 14px;font-family:{Font};font-size:15px;line-height:1.7;color:{Text}\">{html}</p>";
        }

        private static string Note(string html)
        {
            return $"<p style=\"margin:18px 0 0;padding:12px 14px;background:{Soft};border-radius:10px;font-family:{Font};font-size:13px;line-height:1.6;color:{Muted}\">{html}</p>";
        }

        // טבלת פרטים: שורה לכל פרט, בלי שורות ריקות
        private static string DetailsTable(params (string Label, string? Value)[] rows)
        {
            var sb = new StringBuilder();
            sb.Append($"<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"margin:4px 0 18px;background:{Soft};border:1px solid {Border};border-radius:12px;border-collapse:separate\">");
            var visible = rows.Where(r => !string.IsNullOrEmpty(r.Value)).ToList();
            for (var i = 0; i < visible.Count; i++)
            {
                var border = i < visible.Count - 1 ? $"border-bottom:1px solid {Border};" : "";
                sb.Append("<tr>")
                  .Append($"<td style=\"{border}padding:11px 16px;width:110px;font-family:{Font};font-size:13px;color:{Muted};vertical-align:top\">{visible[i].Label}</td>")
                  .Append($"<td style=\"{border}padding:11px 16px;font-family:{Font};font-size:14px;color:{Text}\">{visible[i].Value}</td>")
                  .Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        // כפתור שנראה כמו כפתור גם ב-Outlook (טבלה עם רקע, ולא רק קישור מעוצב)
        private static string Button(string url, string text)
        {
            var href = WebUtility.HtmlEncode(url);
            return "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"margin:8px 0 6px\"><tr>"
                 + $"<td align=\"center\" bgcolor=\"{Primary}\" style=\"border-radius:12px;background:{Primary}\">"
                 + $"<a href=\"{href}\" target=\"_blank\" style=\"display:inline-block;padding:13px 28px;font-family:{Font};font-size:15px;font-weight:700;color:#ffffff;text-decoration:none;border-radius:12px\">{text}</a>"
                 + "</td></tr></table>";
        }

        private static string FallbackLink(string url)
        {
            var href = WebUtility.HtmlEncode(url);
            return $"<p style=\"margin:18px 0 0;font-family:{Font};font-size:12px;line-height:1.6;color:{Muted}\">אם הכפתור לא נפתח, אפשר להעתיק את הקישור לדפדפן:<br>"
                 + $"<a href=\"{href}\" dir=\"ltr\" style=\"color:{Primary};word-break:break-all\">{href}</a></p>";
        }

        // המסגרת של כל מייל: רקע אפור בהיר, פס עליון עם שם האתר, כרטיס לבן ושורת סיום
        private static string Layout(string preheader, string body)
        {
            return "<!DOCTYPE html><html lang=\"he\" dir=\"rtl\"><head><meta charset=\"utf-8\">"
                 + "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">"
                 + "<meta name=\"color-scheme\" content=\"light\"><meta name=\"supported-color-schemes\" content=\"light\">"
                 + "<title>מנה-לי</title></head>"
                 + $"<body style=\"margin:0;padding:0;background:{PageBg}\">"
                 // טקסט מקדים: מופיע בתיבת הדואר ליד הנושא, ולא בתוך המייל
                 + $"<div style=\"display:none;max-height:0;overflow:hidden;opacity:0;color:{PageBg}\">{WebUtility.HtmlEncode(preheader)}</div>"
                 + $"<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" bgcolor=\"{PageBg}\" style=\"background:{PageBg}\" dir=\"rtl\"><tr><td align=\"center\" style=\"padding:28px 12px\">"
                 + "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"max-width:560px\">"
                 // פס עליון
                 + $"<tr><td bgcolor=\"{HeaderBg}\" style=\"background:{HeaderBg};background-image:{HeaderGradient};border-radius:18px 18px 0 0;padding:24px 28px\" dir=\"rtl\" align=\"right\">"
                 + "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr>"
                 + "<td style=\"width:44px;height:44px;background:#7c83ff;border-radius:12px;text-align:center;vertical-align:middle;font-size:22px;line-height:44px\">🍽️</td>"
                 + $"<td style=\"padding-right:12px;font-family:{Font};color:#ffffff\">"
                 + "<div style=\"font-size:20px;font-weight:800;line-height:1.2\">מנה-לי</div>"
                 + "<div style=\"font-size:13px;color:#c7d2fe\">ארוחות משותפות</div>"
                 + "</td></tr></table></td></tr>"
                 // גוף המייל
                 + $"<tr><td bgcolor=\"#ffffff\" style=\"background:#ffffff;border:1px solid {Border};border-top:0;border-radius:0 0 18px 18px;padding:30px 28px 28px\" dir=\"rtl\" align=\"right\">"
                 + body
                 + "</td></tr>"
                 // שורת סיום
                 + $"<tr><td align=\"center\" style=\"padding:20px 12px 0;font-family:{Font};font-size:12px;line-height:1.6;color:{Muted}\">"
                 + "כל אחת מביאה מנה, כולן נהנות<br>המייל נשלח אוטומטית ממנה-לי, אין צורך להשיב לו."
                 + "</td></tr>"
                 + "</table></td></tr></table></body></html>";
        }

        private static string MealTypeName(MealType type) => type switch
        {
            MealType.Dairy => "חלבית",
            MealType.Meat => "בשרית",
            _ => "פרווה",
        };

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
