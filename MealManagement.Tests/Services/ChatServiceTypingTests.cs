using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using MealManagement.Service.Services;
using MealManagement.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MealManagement.Tests.Services
{
    public class ChatServiceTypingTests
    {
        private readonly Mock<IChatMessageRepository> _messages = new();
        private readonly Mock<IMealRepository> _meals = new();
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IChatTypingStore> _typing = new();

        private readonly ChatService _service;

        public ChatServiceTypingTests()
        {
            _service = new ChatService(
                _messages.Object,
                _meals.Object,
                _users.Object,
                _unitOfWork.Object,
                _typing.Object,
                TestMapper.Create(),
                NullLogger<ChatService>.Instance);
        }

        private void GiveAccess(int mealId, int userId)
        {
            _meals.Setup(m => m.HasAccessAsync(mealId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        }

        [Fact]
        public async Task SetTypingAsync_WithText_SavesNameAndText()
        {
            GiveAccess(1, 2);
            _users.Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new User { Id = 2, FullName = "רחל לוי" });

            await _service.SetTypingAsync(1, new UpdateTypingDto { Text = "כן, צריך" }, 2);

            _typing.Verify(t => t.Set(1, 2, "רחל לוי", "כן, צריך"), Times.Once);
        }

        [Fact]
        public async Task SetTypingAsync_UserHidesText_SavesOnlyThatSheIsTyping()
        {
            GiveAccess(1, 2);
            _users.Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new User { Id = 2, FullName = "רחל לוי", ShowTypingText = false });

            // גם אם דפדפן ישן שלח את הטקסט, הוא לא נשמר
            await _service.SetTypingAsync(1, new UpdateTypingDto { Text = "סוד" }, 2);

            _typing.Verify(t => t.Set(1, 2, "רחל לוי", ""), Times.Once);
        }

        [Fact]
        public async Task SetTypingAsync_IsTypingWithoutText_SavesOnlyThatSheIsTyping()
        {
            GiveAccess(1, 2);
            _users.Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new User { Id = 2, FullName = "רחל לוי", ShowTypingText = false });

            await _service.SetTypingAsync(1, new UpdateTypingDto { IsTyping = true }, 2);

            _typing.Verify(t => t.Set(1, 2, "רחל לוי", ""), Times.Once);
        }

        [Fact]
        public async Task SetTypingAsync_IsTypingFalse_ClearsEvenWithText()
        {
            GiveAccess(1, 2);

            await _service.SetTypingAsync(1, new UpdateTypingDto { Text = "abc", IsTyping = false }, 2);

            _typing.Verify(t => t.Clear(1, 2), Times.Once);
            _typing.Verify(t => t.Set(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SetTypingAsync_EmptyText_ClearsTheEntry(string? text)
        {
            GiveAccess(1, 2);

            await _service.SetTypingAsync(1, new UpdateTypingDto { Text = text }, 2);

            _typing.Verify(t => t.Clear(1, 2), Times.Once);
            _typing.Verify(t => t.Set(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SetTypingAsync_NotParticipant_ThrowsForbiddenAndSavesNothing()
        {
            _meals.Setup(m => m.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.SetTypingAsync(1, new UpdateTypingDto { Text = "שלום" }, 2));

            _typing.Verify(t => t.Set(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetTypingAsync_NotParticipant_ThrowsForbidden()
        {
            _meals.Setup(m => m.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetTypingAsync(1, 2));
            _typing.Verify(t => t.GetActive(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SendAsync_ClearsWhatWasBeingTyped()
        {
            GiveAccess(1, 2);
            _users.Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new User { Id = 2, FullName = "רחל לוי" });

            await _service.SendAsync(1, new SendMessageDto { Content = "שלום" }, 2);

            _typing.Verify(t => t.Clear(1, 2), Times.Once);
        }
    }
}
