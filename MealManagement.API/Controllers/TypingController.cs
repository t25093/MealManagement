using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    // "כותבת עכשיו" בצ'אט: משתתפות רואות את הטקסט שאחרות מקלידות, עוד לפני השליחה
    [ApiController]
    [Route("api/meals/{mealId:int}/typing")]
    [Authorize]
    public class TypingController : ControllerBase
    {
        private readonly IChatService _chat;

        public TypingController(IChatService chat)
        {
            _chat = chat;
        }

        // מי כותבת עכשיו (בלי המשתמשת עצמה), עם הטקסט שהקלידה
        [HttpGet]
        [ProducesResponseType(typeof(List<TypingStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TypingStatusDto>>> Get(int mealId, CancellationToken ct)
        {
            return Ok(await _chat.GetTypingAsync(mealId, User.GetUserId(), ct));
        }

        // הטקסט שבשדה ההודעה כרגע. טקסט ריק = הפסיקה לכתוב
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int mealId, UpdateTypingDto dto, CancellationToken ct)
        {
            await _chat.SetTypingAsync(mealId, dto.Text, User.GetUserId(), ct);
            return NoContent();
        }
    }
}
