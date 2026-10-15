using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/meals/{mealId:int}/messages")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IChatService _chat;

        public MessagesController(IChatService chat)
        {
            _chat = chat;
        }

        // בלי afterId: ההודעות האחרונות בעמודים. עם afterId: רק הודעות חדשות (polling)
        [HttpGet]
        [ProducesResponseType(typeof(List<ChatMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ChatMessageDto>>> GetMessages(
            int mealId,
            [FromQuery] int? afterId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            return Ok(await _chat.GetMessagesAsync(mealId, afterId, page, pageSize, User.GetUserId(), ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChatMessageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChatMessageDto>> Send(int mealId, SendMessageDto dto, CancellationToken ct)
        {
            var message = await _chat.SendAsync(mealId, dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetMessages), new { mealId }, message);
        }
    }
}
