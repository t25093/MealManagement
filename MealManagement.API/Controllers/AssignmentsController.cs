using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/meals/{mealId:int}")]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly IUserDishAssignmentService _assignments;

        public AssignmentsController(IUserDishAssignmentService assignments)
        {
            _assignments = assignments;
        }

        // ⭐ לקיחת מנה: כאן מתרחשת התחרות על המשאב. התנגשות על xmin ← 409
        [HttpPost("dishes/{mealDishId:int}/assignments")]
        [ProducesResponseType(typeof(AssignmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AssignmentDto>> Take(int mealId, int mealDishId, CreateAssignmentDto dto, CancellationToken ct)
        {
            var assignment = await _assignments.TakeAsync(mealId, mealDishId, dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetByMeal), new { mealId }, assignment);
        }

        [HttpGet("assignments")]
        [ProducesResponseType(typeof(List<AssignmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AssignmentDto>>> GetByMeal(int mealId, CancellationToken ct)
        {
            return Ok(await _assignments.GetByMealAsync(mealId, User.GetUserId(), ct));
        }

        [HttpDelete("assignments/{assignmentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel(int mealId, int assignmentId, CancellationToken ct)
        {
            await _assignments.CancelAsync(mealId, assignmentId, User.GetUserId(), ct);
            return NoContent();
        }
    }
}
