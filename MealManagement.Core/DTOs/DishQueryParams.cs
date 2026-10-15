using System.ComponentModel.DataAnnotations;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class DishQueryParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
        }

        // חיפוש לפי שם מאכל
        [StringLength(100)]
        public string? Search { get; set; }

        // סינון לפי סוג מנה
        public DishCategory? Category { get; set; }

        // סינון לפי כשרות (למשל להציג רק מאכלים שמתאימים לארוחה חלבית)
        public MealType? KosherType { get; set; }

        // "name" או "category"
        public string? SortBy { get; set; }
    }
}