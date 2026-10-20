using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly IMealService _meals;

        public UsersController(IUserService users, IMealService meals)
        {
            _users = users;
            _meals = meals;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetMe(CancellationToken ct)
        {
            return Ok(await _users.GetByIdAsync(User.GetUserId(), ct));
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            return Ok(await _users.GetAllAsync(page, pageSize, ct));
        }

        // פרטי משתמשת אחת: למנהלת המערכת בלבד
        [HttpGet("{userId:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetById(int userId, CancellationToken ct)
        {
            return Ok(await _users.GetByIdAsync(userId, ct));
        }

        // הארוחות של משתמשת מסוימת (בבעלותה או שהיא משתתפת בהן): למנהלת המערכת בלבד.
        // אותם פרמטרים כמו ב-GET api/meals: role, search, sortBy, page, pageSize
        [HttpGet("{userId:int}/meals")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(PagedResult<MealSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<MealSummaryDto>>> GetUserMeals(
            int userId,
            [FromQuery] MealQueryParams query,
            CancellationToken ct)
        {
            // זורק NotFound אם אין משתמשת כזו
            await _users.GetByIdAsync(userId, ct);
            return Ok(await _meals.GetMyMealsAsync(query, userId, ct));
        }

        // מחיקת החשבון של המשתמשת המחוברת
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteMe(CancellationToken ct)
        {
            var userId = User.GetUserId();
            await _users.DeleteAsync(userId, userId, User.IsInRole("Admin"), ct);
            return NoContent();
        }

        [HttpDelete("{userId:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int userId, CancellationToken ct)
        {
            await _users.DeleteAsync(userId, User.GetUserId(), isAdmin: true, ct);
            return NoContent();
        }
    }
}