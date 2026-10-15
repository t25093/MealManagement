using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/meals/{mealId:int}/recipes")]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipes;

        public RecipesController(IRecipeService recipes)
        {
            _recipes = recipes;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<RecipeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<RecipeDto>>> GetByMeal(int mealId, CancellationToken ct)
        {
            return Ok(await _recipes.GetByMealAsync(mealId, User.GetUserId(), ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecipeDto>> Create(int mealId, CreateRecipeDto dto, CancellationToken ct)
        {
            var created = await _recipes.CreateAsync(mealId, dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetByMeal), new { mealId }, created);
        }

        // הכותבת או בעלת הארוחה (נבדק ב-Service)
        [HttpDelete("{recipeId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int mealId, int recipeId, CancellationToken ct)
        {
            await _recipes.DeleteAsync(mealId, recipeId, User.GetUserId(), ct);
            return NoContent();
        }
    }
}
