namespace MealManagement.Core.Services
{
    // שליחה טכנית של מייל אחד (Brevo, או רישום ביומן כשאין מפתח)
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlContent, CancellationToken ct = default);
    }
}
