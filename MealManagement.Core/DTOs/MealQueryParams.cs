using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class MealQueryParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        // תקרה: גם אם הלקוח מבקש 1000, יקבל לכל היותר 50
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
        }

        // חיפוש לפי שם ארוחה
        [StringLength(100)]
        public string? Search { get; set; }

        // "date" או "name"
        public string? SortBy { get; set; }

        // "owner" = ארוחות שלי, "participant" = ארוחות שאני משתתפת בהן, ריק = הכול
        public string? Role { get; set; }
    }
}