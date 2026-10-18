using MealManagement.Core.Entities;
using MealManagement.Core.Services;
using MealManagement.Core.Settings;
using MealManagement.Service.Email;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text.Json;

namespace MealManagement.Tests.Email
{
    public class EmailTests
    {
        private static readonly IOptions<EmailSettings> Settings = Options.Create(new EmailSettings
        {
            ApiKey = "test-key",
            SenderEmail = "sender@test.com",
            SenderName = "מנה-לי",
            ClientUrl = "https://client.test/"
        });

        // HttpMessageHandler מדומה: שומר את הבקשה ומחזיר תשובה קבועה
        private class CaptureHandler : HttpMessageHandler
        {
            public HttpRequestMessage? Request { get; private set; }
            public string? Body { get; private set; }
            public HttpStatusCode Status { get; init; } = HttpStatusCode.Created;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            {
                Request = request;
                Body = await request.Content!.ReadAsStringAsync(ct);
                return new HttpResponseMessage(Status) { Content = new StringContent("{\"message\":\"bad key\"}") };
            }
        }

        [Fact]
        public async Task BrevoEmailSender_SendsRequestInBrevoFormat()
        {
            // Arrange
            var handler = new CaptureHandler();
            var sender = new BrevoEmailSender(new HttpClient(handler), Settings);

            // Act
            await sender.SendAsync("dina@test.com", "נושא", "<p>תוכן</p>");

            // Assert: הכתובת, המפתח וגוף הבקשה לפי ה-API של Brevo
            Assert.Equal(HttpMethod.Post, handler.Request!.Method);
            Assert.Equal("https://api.brevo.com/v3/smtp/email", handler.Request.RequestUri!.ToString());
            Assert.Equal("test-key", handler.Request.Headers.GetValues("api-key").Single());

            using var json = JsonDocument.Parse(handler.Body!);
            var root = json.RootElement;
            Assert.Equal("sender@test.com", root.GetProperty("sender").GetProperty("email").GetString());
            Assert.Equal("dina@test.com", root.GetProperty("to")[0].GetProperty("email").GetString());
            Assert.Equal("נושא", root.GetProperty("subject").GetString());
            Assert.Equal("<p>תוכן</p>", root.GetProperty("htmlContent").GetString());
        }

        [Fact]
        public async Task BrevoEmailSender_ErrorFromBrevo_Throws()
        {
            var sender = new BrevoEmailSender(new HttpClient(new CaptureHandler { Status = HttpStatusCode.Unauthorized }), Settings);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sender.SendAsync("a@test.com", "s", "b"));
            Assert.Contains("401", ex.Message);
        }

        [Fact]
        public async Task SendPasswordResetAsync_LinkContainsClientUrlAndToken()
        {
            // Arrange
            var emailSender = new Mock<IEmailSender>();
            string? html = null;
            emailSender.Setup(s => s.SendAsync("dina@test.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .Callback<string, string, string, CancellationToken>((_, _, body, _) => html = body);
            var notifications = new EmailNotifications(emailSender.Object, Settings, NullLogger<EmailNotifications>.Instance);

            // Act
            await notifications.SendPasswordResetAsync(new User { FullName = "דינה", Email = "dina@test.com" }, "tok_123");

            // Assert
            Assert.Contains("https://client.test/reset-password?token=tok_123", html);
        }

        [Fact]
        public async Task SendMealInvitationAsync_SenderFails_DoesNotThrow()
        {
            // Arrange: Brevo נכשל
            var emailSender = new Mock<IEmailSender>();
            emailSender.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Brevo returned 401"));
            var notifications = new EmailNotifications(emailSender.Object, Settings, NullLogger<EmailNotifications>.Instance);
            var meal = new Meal { Id = 7, Name = "ארוחת שבת", EventDate = new DateTime(2026, 12, 4, 15, 0, 0, DateTimeKind.Utc) };

            // Act + Assert: הכישלון נרשם ביומן, והפעולה שקראה לשליחה ממשיכה
            await notifications.SendMealInvitationAsync("new@test.com", meal, "שרה", isRegistered: false);
            emailSender.Verify(s => s.SendAsync("new@test.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendMealInvitationAsync_RegisteredUser_LinksToMealInIsraelTime()
        {
            // Arrange
            var emailSender = new Mock<IEmailSender>();
            string? html = null;
            emailSender.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .Callback<string, string, string, CancellationToken>((_, _, body, _) => html = body);
            var notifications = new EmailNotifications(emailSender.Object, Settings, NullLogger<EmailNotifications>.Instance);
            var meal = new Meal { Id = 7, Name = "ארוחת שבת", EventDate = new DateTime(2026, 12, 4, 15, 0, 0, DateTimeKind.Utc) };

            // Act
            await notifications.SendMealInvitationAsync("dina@test.com", meal, "שרה", isRegistered: true);

            // Assert: 15:00 UTC בדצמבר הוא 17:00 בשעון ישראל
            Assert.Contains("https://client.test/meals/7", html);
            Assert.Contains("04/12/2026 בשעה 17:00", html);
        }
    }
}
