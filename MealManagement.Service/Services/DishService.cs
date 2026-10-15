using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DishService> _logger;

        public DishService(
            IDishRepository dishes,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DishService> logger)
        {
            _dishes = dishes;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<DishDto>> GetAllAsync(DishQueryParams query, CancellationToken ct = default)
        {
            query.Page = Math.Max(query.Page, 1);

            var result = await _dishes.GetPagedAsync(query, ct);

            return new PagedResult<DishDto>
            {
                Items = _mapper.Map<List<DishDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<DishDto> GetByIdAsync(int dishId, CancellationToken ct = default)
        {
            var dish = await GetDishOrThrowAsync(dishId, ct);
            return _mapper.Map<DishDto>(dish);
        }

        public async Task<DishDto> CreateAsync(CreateDishDto dto, int currentUserId, CancellationToken ct = default)
        {
            var name = dto.Name.Trim();

            if (await _dishes.ExistsByNameAsync(name, dto.Category, ct))
                throw new ConflictException($"המאכל \"{name}\" כבר קיים בקטגוריה הזו");

            var dish = _mapper.Map<Dish>(dto);
            dish.Name = name;
            dish.CreatedByUserId = currentUserId;

            await _dishes.AddAsync(dish, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Dish {DishId} created by user {UserId}", dish.Id, currentUserId);

            return _mapper.Map<DishDto>(dish);
        }

        public async Task UpdateAsync(int dishId, CreateDishDto dto, CancellationToken ct = default)
        {
            var dish = await GetDishOrThrowAsync(dishId, ct);
            var name = dto.Name.Trim();

            var nameOrCategoryChanged =
                !string.Equals(dish.Name, name, StringComparison.OrdinalIgnoreCase)
                || dish.Category != dto.Category;

            if (nameOrCategoryChanged && await _dishes.ExistsByNameAsync(name, dto.Category, ct))
                throw new ConflictException($"המאכל \"{name}\" כבר קיים בקטגוריה הזו");

            _mapper.Map(dto, dish);
            dish.Name = name;

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Dish {DishId} updated", dishId);
        }

        public async Task DeleteAsync(int dishId, CancellationToken ct = default)
        {
            var dish = await GetDishOrThrowAsync(dishId, ct);

            if (await _dishes.IsInUseAsync(dishId, ct))
                throw new ConflictException("לא ניתן למחוק מאכל שנמצא בשימוש בארוחה");

            _dishes.Remove(dish);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Dish {DishId} deleted", dishId);
        }

        private async Task<Dish> GetDishOrThrowAsync(int dishId, CancellationToken ct)
        {
            return await _dishes.GetByIdAsync(dishId, ct)
                ?? throw new NotFoundException("המאכל לא נמצא");
        }
    }
}