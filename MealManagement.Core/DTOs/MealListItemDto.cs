using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class MealListItemDto
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public MealListType ListType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsDone { get; set; }
        public int? DoneByUserId { get; set; }
        public string? DoneByName { get; set; }
        public DateTime? DoneAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
