using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Email
{
    // כשאין מפתח של Brevo (במחשב המקומי): המייל לא נשלח, אלא נרשם ביומן עם הקישורים שבו
    public class LogEmailSender : IEmailSender
    {
        private readonly ILogger<LogEmailSender> _logger;

        public LogEmailSender(ILogger<LogEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string toEmail, string subject, string htmlContent, CancellationToken ct = default)
        {
            _logger.LogInformation("Email not sent (no Email:ApiKey). To: {To}, Subject: {Subject}, Body: {Body}",
                toEmail, subject, htmlContent);
            return Task.CompletedTask;
        }
    }
}
