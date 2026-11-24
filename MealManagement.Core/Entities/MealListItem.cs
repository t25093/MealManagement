using MealManagement.Core.Enums;

namespace MealManagement.Core.Entities
{
    // פריט ברשימת הקניות או ברשימת הדברים להביא של ארוחה
    public class MealListItem
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public Meal Meal { get; set; }
        public MealListType ListType { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }

        // נקנה / הובא, ומי סימנה
        public bool IsDone { get; set; }
        public int? DoneByUserId { get; set; }
        public User? DoneBy { get; set; }
        public DateTime? DoneAt { get; set; }

        public int? CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
