using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IChatMessageRepository
    {
        // AsNoTracking: ההודעות האחרונות, בעמודים
        Task<List<ChatMessage>> GetLatestAsync(int mealId, int page, int pageSize, CancellationToken ct = default);

        // AsNoTracking: רק הודעות חדשות מאז Id מסוים (בשביל polling)
        Task<List<ChatMessage>> GetAfterAsync(int mealId, int afterId, CancellationToken ct = default);

        Task AddAsync(ChatMessage message, CancellationToken ct = default);
    }
}
