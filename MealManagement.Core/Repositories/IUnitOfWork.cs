namespace MealManagement.Core.Repositories
{
    // שמירה אחת לכל הפעולה, בטרנזקציה אחת.
    // המימוש (בפרויקט Data) מתרגם התנגשויות ל-ConflictException.
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
