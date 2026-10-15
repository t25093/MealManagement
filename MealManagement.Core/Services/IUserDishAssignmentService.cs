using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IUserDishAssignmentService
    {
        // ⭐ לקיחת מנה: כאן מתרחשת התחרות על המשאב
        Task<AssignmentDto> TakeAsync(int mealId, int mealDishId, CreateAssignmentDto dto, int currentUserId, CancellationToken ct = default);

        Task<List<AssignmentDto>> GetByMealAsync(int mealId, int currentUserId, CancellationToken ct = default);

        // ביטול: מחזיר את הכמות למנה (גם זה עדכון מוגן)
        Task CancelAsync(int mealId, int assignmentId, int currentUserId, CancellationToken ct = default);
    }
}
