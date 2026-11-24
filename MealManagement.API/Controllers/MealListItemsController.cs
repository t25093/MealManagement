using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    // רשימת הקניות ורשימת הדברים להביא של ארוחה
    [ApiController]
    [Route("api/meals/{mealId:int}/list-items")]
    [Authorize]
    public class MealListItemsController : ControllerBase
    {
        private readonly IMealListItemService _items;

        public MealListItemsController(IMealListItemService items)
        {
            _items = items;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MealListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<MealListItemDto>>> GetByMeal(int mealId, CancellationToken ct)
        {
            return Ok(await _items.GetByMealAsync(mealId, User.GetUserId(), ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(MealListItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealListItemDto>> Create(int mealId, CreateMealListItemDto dto, CancellationToken ct)
        {
            var created = await _items.CreateAsync(mealId, dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetByMeal), new { mealId }, created);
        }

        // עריכת שם או הערה, וסימון "נקנה" / "הובא"
        [HttpPatch("{itemId:int}")]
        [ProducesResponseType(typeof(MealListItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealListItemDto>> Update(int mealId, int itemId, UpdateMealListItemDto dto, CancellationToken ct)
        {
            return Ok(await _items.UpdateAsync(mealId, itemId, dto, User.GetUserId(), ct));
        }

        [HttpDelete("{itemId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int mealId, int itemId, CancellationToken ct)
        {
            await _items.DeleteAsync(mealId, itemId, User.GetUserId(), ct);
            return NoContent();
        }
    }
}
