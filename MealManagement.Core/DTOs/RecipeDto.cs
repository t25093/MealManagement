namespace MealManagement.Core.DTOs
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public int? DishId { get; set; }
        public string? DishName { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}