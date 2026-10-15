namespace MealManagement.Core.DTOs
{
    // עטיפה גנרית לכל רשימה שמוחזרת בעמודים
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // מחושב, לא נשמר: כמה עמודים יש בסך הכול
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}