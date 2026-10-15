using System.ComponentModel.DataAnnotations;
using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/meals/{mealId:int}/dishes")]
    [Authorize]
    public class MealDishesController : ControllerBase
    {
        private readonly IMealDishService _mealDishes;

        public MealDishesController(IMealDishService mealDishes)
        {
            _mealDishes = mealDishes;
        }

        // status=all (ברירת מחדל) או status=open (רק מנות שנותרה בהן כמות)
        [HttpGet]
        [ProducesResponseType(typeof(List<MealDishDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<MealDishDto>>> GetByMeal(
            int mealId,
            [FromQuery, RegularExpression("^(all|open)$", ErrorMessage = "status חייב להיות all או open")] string status = "all",
            CancellationToken ct = default)
        {
            var onlyOpen = status == "open";
            return Ok(await _mealDishes.GetByMealAsync(mealId, onlyOpen, User.GetUserId(), ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(MealDishDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealDishDto>> Add(int mealId, CreateMealDishDto dto, CancellationToken ct)
        {
            var created = await _mealDishes.AddAsync(mealId, dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetByMeal), new { mealId }, created);
        }

        [HttpDelete("{mealDishId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Remove(int mealId, int mealDishId, CancellationToken ct)
        {
            await _mealDishes.RemoveAsync(mealId, mealDishId, User.GetUserId(), ct);
            return NoContent();
        }
    }
}
