using MealManagement.Core.Services;
using MealManagement.Core.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MealManagement.Service.Email
{
    // שליחה דרך ה-API של Brevo ב-HTTPS. ב-Render החינמי שליחה רגילה (SMTP) חסומה
    public class BrevoEmailSender : IEmailSender
    {
        private readonly HttpClient _http;
        private readonly EmailSettings _settings;

        public BrevoEmailSender(HttpClient http, IOptions<EmailSettings> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlContent, CancellationToken ct = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Content = JsonContent.Create(new
                {
                    sender = new { name = _settings.SenderName, email = _settings.SenderEmail },
                    to = new[] { new { email = toEmail } },
                    subject,
                    htmlContent
                })
            };
            request.Headers.Add("api-key", _settings.ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException($"Brevo returned {(int)response.StatusCode}: {body}");
            }
        }
    }
}
