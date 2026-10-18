using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IMealInvitationRepository
    {
        // עם מעקב, כולל הארוחה: משמש בהרשמה כדי להפוך הזמנות להשתתפות
        Task<List<MealInvitation>> GetByEmailAsync(string email, CancellationToken ct = default);
        void RemoveRange(IEnumerable<MealInvitation> invitations);
    }
}
