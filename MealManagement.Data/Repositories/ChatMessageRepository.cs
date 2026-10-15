using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _context;

        public ChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetLatestAsync(int mealId, int page, int pageSize, CancellationToken ct = default)
        {
            var messages = await _context.ChatMessages
                .AsNoTracking()
                .Include(c => c.Sender)
                .Where(c => c.MealId == mealId)
                .OrderByDescending(c => c.SentAt)
                .ThenByDescending(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            messages.Reverse();
            return messages;
        }

        public async Task<List<ChatMessage>> GetAfterAsync(int mealId, int afterId, CancellationToken ct = default)
        {
            return await _context.ChatMessages
                .AsNoTracking()
                .Include(c => c.Sender)
                .Where(c => c.MealId == mealId && c.Id > afterId)
                .OrderBy(c => c.Id)
                .ToListAsync(ct);
        }

        public async Task AddAsync(ChatMessage message, CancellationToken ct = default)
        {
            await _context.ChatMessages.AddAsync(message, ct);
        }
    }
}