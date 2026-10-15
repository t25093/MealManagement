using MealManagement.Core.DTOs;

namespace MealManagement.Core.Services
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetByMealAsync(int mealId, int currentUserId, CancellationToken ct = default);
        Task<RecipeDto> CreateAsync(int mealId, CreateRecipeDto dto, int currentUserId, CancellationToken ct = default);

        // הכותבת או בעלת הארוחה
        Task DeleteAsync(int mealId, int recipeId, int currentUserId, CancellationToken ct = default);
    }
}
