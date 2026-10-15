using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;

namespace MealManagement.Core.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe?> GetByIdAsync(int recipeId, CancellationToken ct = default);

        // AsNoTracking, כולל שם הכותבת ושם המאכל
        Task<List<Recipe>> GetByMealAsync(int mealId, CancellationToken ct = default);

        Task AddAsync(Recipe recipe, CancellationToken ct = default);
        void Remove(Recipe recipe);
    }
}
