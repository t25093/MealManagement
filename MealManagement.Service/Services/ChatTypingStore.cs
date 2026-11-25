using System.Collections.Concurrent;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;

namespace MealManagement.Service.Services
{
    // "כותבת עכשיו" בזיכרון של השרת (Singleton). לא נשמר בבסיס הנתונים:
    // זה מידע של שניות, והוא נעלם לבד אם הדפדפן נסגר באמצע הקלדה.
    public class ChatTypingStore : IChatTypingStore
    {
        // רשומה שלא התעדכנה זמן כזה נחשבת כאילו הפסיקה לכתוב
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(8);

        private readonly ConcurrentDictionary<(int MealId, int UserId), Entry> _entries = new();
        private readonly TimeProvider _time;

        public ChatTypingStore(TimeProvider time)
        {
            _time = time;
        }

        public void Set(int mealId, int userId, string userName, string text)
        {
            var now = _time.GetUtcNow();
            _entries[(mealId, userId)] = new Entry(userName, text, now);
            RemoveExpired(now);
        }

        public void Clear(int mealId, int userId)
        {
            _entries.TryRemove((mealId, userId), out _);
        }

        public List<TypingStatusDto> GetActive(int mealId, int exceptUserId)
        {
            var since = _time.GetUtcNow() - Lifetime;

            return _entries
                .Where(e => e.Key.MealId == mealId && e.Key.UserId != exceptUserId && e.Value.UpdatedAt > since)
                .OrderBy(e => e.Value.UpdatedAt)
                .Select(e => new TypingStatusDto { UserId = e.Key.UserId, Name = e.Value.UserName, Text = e.Value.Text })
                .ToList();
        }

        // ניקוי רשומות ישנות, כדי שהזיכרון לא יגדל עם הזמן
        private void RemoveExpired(DateTimeOffset now)
        {
            foreach (var entry in _entries)
            {
                if (now - entry.Value.UpdatedAt > Lifetime)
                    _entries.TryRemove(entry);
            }
        }

        private sealed record Entry(string UserName, string Text, DateTimeOffset UpdatedAt);
    }
}
