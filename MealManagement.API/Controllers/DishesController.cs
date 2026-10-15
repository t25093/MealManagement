using MealManagement.API.Extensions;
using MealManagement.Core.DTOs;
using MealManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealManagement.API.Controllers
{
    [ApiController]
    [Route("api/dishes")]
    [Authorize]
    public class DishesController : ControllerBase
    {
        private readonly IDishService _dishes;

        public DishesController(IDishService dishes)
        {
            _dishes = dishes;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DishDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<DishDto>>> GetAll([FromQuery] DishQueryParams query, CancellationToken ct)
        {
            return Ok(await _dishes.GetAllAsync(query, ct));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DishDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishDto>> GetById(int id, CancellationToken ct)
        {
            return Ok(await _dishes.GetByIdAsync(id, ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(DishDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DishDto>> Create(CreateDishDto dto, CancellationToken ct)
        {
            var created = await _dishes.CreateAsync(dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CreateDishDto dto, CancellationToken ct)
        {
            await _dishes.UpdateAsync(id, dto, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _dishes.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
