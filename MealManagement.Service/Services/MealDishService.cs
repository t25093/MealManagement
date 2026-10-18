using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class MealDishService : IMealDishService
    {
        private readonly IMealDishRepository _mealDishes;
        private readonly IMealRepository _meals;
        private readonly IDishRepository _dishes;
        private readonly IUserDishAssignmentRepository _assignments;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MealDishService> _logger;

        public MealDishService(
            IMealDishRepository mealDishes,
            IMealRepository meals,
            IDishRepository dishes,
            IUserDishAssignmentRepository assignments,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MealDishService> logger)
        {
            _mealDishes = mealDishes;
            _meals = meals;
            _dishes = dishes;
            _assignments = assignments;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ===== צעד 1: תצוגה =====

        public async Task<List<MealDishDto>> GetByMealAsync(int mealId, bool onlyOpen, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            var mealDishes = await _mealDishes.GetByMealAsync(mealId, onlyOpen, ct);

            var assignmentsByMealDish = (await _assignments.GetByMealAsync(mealId, ct))
                .GroupBy(a => a.MealDishId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<MealDishDto>();
            foreach (var mealDish in mealDishes)
            {
                var dto = _mapper.Map<MealDishDto>(mealDish);

                if (assignmentsByMealDish.TryGetValue(mealDish.Id, out var assignments))
                    dto.Assignments = _mapper.Map<List<AssignmentDto>>(assignments);

                result.Add(dto);
            }

            return result;
        }

        // ===== צעד 1: הוספת מנה =====

        public async Task<MealDishDto> AddAsync(int mealId, CreateMealDishDto dto, int currentUserId, CancellationToken ct = default)
        {
            var meal = await _meals.GetForUpdateAsync(mealId, ct)
                ?? throw new NotFoundException("הארוחה לא נמצאה");

            if (!MealPermissions.CanEdit(meal, currentUserId))
                throw new ForbiddenException("רק בעלת הארוחה או משתתפת עם הרשאת עריכה יכולות להוסיף מנות");

            MealDish mealDish;

            if (dto.DishId.HasValue)
            {
                var dish = await _dishes.GetByIdAsync(dto.DishId.Value, ct)
                    ?? throw new NotFoundException("המאכל לא נמצא במאגר");

                if (!KashrutRules.IsAllowed(meal.MealType, dish.KosherType))
                    throw new BusinessRuleException($"המאכל \"{dish.Name}\" אינו מתאים לסוג הכשרות של הארוחה");

                if (meal.MealDishes.Any(md => md.DishId == dish.Id))
                    throw new ConflictException($"המאכל \"{dish.Name}\" כבר נמצא בתפריט");

                mealDish = new MealDish
                {
                    MealId = meal.Id,
                    DishId = dish.Id,
                    Dish = dish,
                    Category = dish.Category,
                    RequiredQuantity = dish.QuantityPerPerson * meal.GuestsCount,
                    AssignedQuantity = 0
                };
            }
            else
            {
                mealDish = new MealDish
                {
                    MealId = meal.Id,
                    DishId = null,
                    Category = dto.Category,
                    RequiredQuantity = 0,
                    AssignedQuantity = 0
                };
            }

            await _mealDishes.AddAsync(mealDish, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("MealDish {MealDishId} added to meal {MealId}", mealDish.Id, mealId);

            return _mapper.Map<MealDishDto>(mealDish);
        }

        // ===== צעד 2: הסרת מנה =====

        public async Task RemoveAsync(int mealId, int mealDishId, int currentUserId, CancellationToken ct = default)
        {
            var mealDish = await _mealDishes.GetForUpdateAsync(mealDishId, ct);

            if (mealDish == null || mealDish.MealId != mealId)
                throw new NotFoundException("המנה לא נמצאה בארוחה הזו");

            if (!MealPermissions.CanEdit(mealDish.Meal, currentUserId))
                throw new ForbiddenException("רק בעלת הארוחה או משתתפת עם הרשאת עריכה יכולות להסיר מנות");

            if (await _mealDishes.HasAssignmentsAsync(mealDishId, ct))
                throw new ConflictException("לא ניתן להסיר מנה שכבר נלקחה. יש לבטל קודם את הלקיחות");

            _mealDishes.Remove(mealDish);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("MealDish {MealDishId} removed from meal {MealId}", mealDishId, mealId);
        }

        // ===== פונקציית עזר =====

        private async Task EnsureAccessAsync(int mealId, int currentUserId, CancellationToken ct)
        {
            if (await _meals.HasAccessAsync(mealId, currentUserId, ct))
                return;

            if (!await _meals.ExistsAsync(mealId, ct))
                throw new NotFoundException("הארוחה לא נמצאה");

            throw new ForbiddenException("אין הרשאה לצפות בארוחה הזו");
        }
    }
}