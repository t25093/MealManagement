using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IUserDishAssignmentRepository
    {
        // עם מעקב, כולל MealDish (לביטול: מחזירים את הכמות למנה)
        Task<UserDishAssignment?> GetForUpdateAsync(int assignmentId, CancellationToken ct = default);

        // עם מעקב: האם המשתמשת כבר לקחה מהמנה הזאת (אם כן, מעדכנים את ההקצאה הקיימת)
        Task<UserDishAssignment?> GetByMealDishAndUserAsync(int mealDishId, int userId, CancellationToken ct = default);

        // עם מעקב: שיבוץ קיים של מוזמנת שעוד לא נרשמה במנה הזאת
        Task<UserDishAssignment?> GetByMealDishAndPendingEmailAsync(int mealDishId, string email, CancellationToken ct = default);

        // עם מעקב: כל השיבוצים שמחכים לאימייל הזה (עוברים למשתמשת כשהיא נרשמת)
        Task<List<UserDishAssignment>> GetPendingByEmailAsync(string email, CancellationToken ct = default);

        // AsNoTracking: כל ההקצאות בארוחה, כולל שם המשתמשת
        Task<List<UserDishAssignment>> GetByMealAsync(int mealId, CancellationToken ct = default);

        Task AddAsync(UserDishAssignment assignment, CancellationToken ct = default);
        void Remove(UserDishAssignment assignment);
    }
}
