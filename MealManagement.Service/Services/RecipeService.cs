using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipes;
        private readonly IMealRepository _meals;
        private readonly IMealDishRepository _mealDishes;
        private readonly IDishRepository _dishes;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(
            IRecipeRepository recipes,
            IMealRepository meals,
            IMealDishRepository mealDishes,
            IDishRepository dishes,
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RecipeService> logger)
        {
            _recipes = recipes;
            _meals = meals;
            _mealDishes = mealDishes;
            _dishes = dishes;
            _users = users;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<RecipeDto>> GetByMealAsync(int mealId, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            var recipes = await _recipes.GetByMealAsync(mealId, ct);
            return _mapper.Map<List<RecipeDto>>(recipes);
        }

        public async Task<RecipeDto> CreateAsync(int mealId, CreateRecipeDto dto, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            Dish? dish = null;

            if (dto.DishId.HasValue)
            {
                dish = await _dishes.GetByIdAsync(dto.DishId.Value, ct)
                    ?? throw new NotFoundException("המאכל לא נמצא במאגר");

                var mealDishes = await _mealDishes.GetByMealAsync(mealId, false, ct);
                if (!mealDishes.Any(md => md.DishId == dish.Id))
                    throw new BusinessRuleException("המאכל אינו חלק מהתפריט של הארוחה הזו");
            }

            var recipe = _mapper.Map<Recipe>(dto);
            recipe.Title = dto.Title.Trim();
            recipe.MealId = mealId;
            recipe.AuthorId = currentUserId;
            recipe.CreatedAt = DateTime.UtcNow;

            await _recipes.AddAsync(recipe, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Recipe {RecipeId} added to meal {MealId} by user {UserId}",
                recipe.Id, mealId, currentUserId);

            var result = _mapper.Map<RecipeDto>(recipe);
            var author = await _users.GetByIdAsync(currentUserId, ct);
            result.AuthorName = author?.FullName ?? string.Empty;
            result.DishName = dish?.Name;
            return result;
        }

        public async Task DeleteAsync(int mealId, int recipeId, int currentUserId, CancellationToken ct = default)
        {
            var recipe = await _recipes.GetByIdAsync(recipeId, ct);

            if (recipe == null || recipe.MealId != mealId)
                throw new NotFoundException("המתכון לא נמצא בארוחה הזו");

            if (recipe.AuthorId != currentUserId)
            {
                var meal = await _meals.GetForUpdateAsync(mealId, ct)
                    ?? throw new NotFoundException("הארוחה לא נמצאה");

                if (meal.OwnerId != currentUserId)
                    throw new ForbiddenException("רק כותבת המתכון או בעלת הארוחה יכולות למחוק אותו");
            }

            _recipes.Remove(recipe);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Recipe {RecipeId} deleted by user {UserId}", recipeId, currentUserId);
        }

        private async Task EnsureAccessAsync(int mealId, int currentUserId, CancellationToken ct)
        {
            if (await _meals.HasAccessAsync(mealId, currentUserId, ct))
                return;

            if (!await _meals.ExistsAsync(mealId, ct))
                throw new NotFoundException("הארוחה לא נמצאה");

            throw new ForbiddenException("אין הרשאה לארוחה הזו");
        }
    }
}
