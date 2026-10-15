using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly AppDbContext _context;

        public RecipeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe?> GetByIdAsync(int recipeId, CancellationToken ct = default)
        {
            return await _context.Recipes.FirstOrDefaultAsync(r => r.Id == recipeId, ct);
        }

        public async Task<List<Recipe>> GetByMealAsync(int mealId, CancellationToken ct = default)
        {
            return await _context.Recipes
                .AsNoTracking()
                .Include(r => r.Author)
                .Include(r => r.Dish)
                .Where(r => r.MealId == mealId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Recipe recipe, CancellationToken ct = default)
        {
            await _context.Recipes.AddAsync(recipe, ct);
        }

        public void Remove(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
        }
    }
}