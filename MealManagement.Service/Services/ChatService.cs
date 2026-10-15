using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class ChatService : IChatService
    {
        private const int MaxPageSize = 50;

        private readonly IChatMessageRepository _messages;
        private readonly IMealRepository _meals;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            IChatMessageRepository messages,
            IMealRepository meals,
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ChatService> logger)
        {
            _messages = messages;
            _meals = meals;
            _users = users;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(int mealId, int? afterId, int page, int pageSize, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            List<ChatMessage> messages;

            if (afterId.HasValue)
            {
                // polling: רק הודעות שנוספו מאז ההודעה האחרונה שהלקוח מכיר
                messages = await _messages.GetAfterAsync(mealId, afterId.Value, ct);
            }
            else
            {
                page = Math.Max(page, 1);
                pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
                messages = await _messages.GetLatestAsync(mealId, page, pageSize, ct);
            }

            return _mapper.Map<List<ChatMessageDto>>(messages);
        }

        public async Task<ChatMessageDto> SendAsync(int mealId, SendMessageDto dto, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            var content = dto.Content.Trim();
            if (content.Length == 0)
                throw new BusinessRuleException("לא ניתן לשלוח הודעה ריקה");

            var message = _mapper.Map<ChatMessage>(dto);
            message.Content = content;
            message.MealId = mealId;
            message.SenderId = currentUserId;
            message.SentAt = DateTime.UtcNow;

            await _messages.AddAsync(message, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Message {MessageId} sent to meal {MealId} by user {UserId}",
                message.Id, mealId, currentUserId);

            var result = _mapper.Map<ChatMessageDto>(message);
            var sender = await _users.GetByIdAsync(currentUserId, ct);
            result.SenderName = sender?.FullName ?? string.Empty;
            return result;
        }

        private async Task EnsureAccessAsync(int mealId, int currentUserId, CancellationToken ct)
        {
            if (await _meals.HasAccessAsync(mealId, currentUserId, ct))
                return;

            if (!await _meals.ExistsAsync(mealId, ct))
                throw new NotFoundException("הארוחה לא נמצאה");

            throw new ForbiddenException("רק משתתפות בארוחה יכולות להשתמש בצ'אט שלה");
        }
    }
}
