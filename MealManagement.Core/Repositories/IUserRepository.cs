using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

        // עם מעקב: המשתמשת שקוד איפוס הסיסמה שלה תואם ל-hash
        Task<User?> GetByPasswordResetTokenHashAsync(string tokenHash, CancellationToken ct = default);
        Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);

        // עם מעקב, כולל הלקיחות (עם המנה) וההודעות: כל מה שצריך לטפל בו לפני מחיקה
        Task<User?> GetForDeleteAsync(int id, CancellationToken ct = default);

        // מנהלת כלשהי במערכת (אליה עוברים המאכלים של משתמשת שנמחקת)
        Task<int?> GetAnyAdminIdAsync(CancellationToken ct = default);

        // מחיקת המשתמשת עם הנתונים שלה, בלי שמירה (השמירה דרך IUnitOfWork):
        // המאכלים שהוסיפה למאגר עוברים ל-dishesNewOwnerId, והארוחות שבבעלותה נמחקות
        Task RemoveWithRelatedDataAsync(User user, int dishesNewOwnerId, CancellationToken ct = default);
    }
}
