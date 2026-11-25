using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IChatService
    {
        // afterId ריק = ההודעות האחרונות בעמודים. afterId מלא = רק חדשות (polling)
        Task<List<ChatMessageDto>> GetMessagesAsync(int mealId, int? afterId, int page, int pageSize, int currentUserId, CancellationToken ct = default);
        Task<ChatMessageDto> SendAsync(int mealId, SendMessageDto dto, int currentUserId, CancellationToken ct = default);

        // "כותבת עכשיו": מעדכן את הטקסט שהמשתמשת מקלידה (ריק = הפסיקה), ומחזיר מי כותבת עכשיו
        Task SetTypingAsync(int mealId, UpdateTypingDto dto, int currentUserId, CancellationToken ct = default);
        Task<List<TypingStatusDto>> GetTypingAsync(int mealId, int currentUserId, CancellationToken ct = default);
    }
}
