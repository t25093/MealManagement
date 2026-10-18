namespace MealManagement.Core.Settings;

// שליחת מיילים דרך Brevo. בלי ApiKey המיילים רק נרשמים ביומן (מתאים למחשב המקומי)
public class EmailSettings
{
    public const string SectionName = "Email";

    public string? ApiKey { get; set; }

    // כתובת שאושרה ב-Brevo ככתובת שולחת
    public string SenderEmail { get; set; } = string.Empty;

    public string SenderName { get; set; } = "מנה-לי";

    // כתובת אתר הלקוח, לקישורים שבמייל. ריק: הכתובת הראשונה ב-Cors:ClientUrl
    public string ClientUrl { get; set; } = string.Empty;
}
