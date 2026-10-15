using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/meals")]
    [Authorize]
    public class MealsController : ControllerBase
    {
        private readonly IMealService _meals;

        public MealsController(IMealService meals)
        {
            _meals = meals;
        }

        // ===== ארוחות =====

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<MealSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<MealSummaryDto>>> GetMine([FromQuery] MealQueryParams query, CancellationToken ct)
        {
            return Ok(await _meals.GetMyMealsAsync(query, User.GetUserId(), ct));
        }

        [HttpGet("{mealId:int}")]
        [ProducesResponseType(typeof(MealDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealDetailsDto>> GetById(int mealId, CancellationToken ct)
        {
            return Ok(await _meals.GetByIdAsync(mealId, User.GetUserId(), ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(MealDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealDetailsDto>> Create(CreateMealDto dto, CancellationToken ct)
        {
            var created = await _meals.CreateAsync(dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { mealId = created.Id }, created);
        }

        [HttpPatch("{mealId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int mealId, UpdateMealDto dto, CancellationToken ct)
        {
            await _meals.UpdateAsync(mealId, dto, User.GetUserId(), ct);
            return NoContent();
        }

        [HttpDelete("{mealId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int mealId, CancellationToken ct)
        {
            await _meals.DeleteAsync(mealId, User.GetUserId(), User.IsInRole("Admin"), ct);
            return NoContent();
        }

        // ===== משתתפות =====

        [HttpPost("{mealId:int}/participants")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDto>> AddParticipant(int mealId, AddParticipantDto dto, CancellationToken ct)
        {
            var participant = await _meals.AddParticipantAsync(mealId, dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { mealId }, participant);
        }

        [HttpDelete("{mealId:int}/participants/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveParticipant(int mealId, int userId, CancellationToken ct)
        {
            await _meals.RemoveParticipantAsync(mealId, userId, User.GetUserId(), ct);
            return NoContent();
        }
    }
}