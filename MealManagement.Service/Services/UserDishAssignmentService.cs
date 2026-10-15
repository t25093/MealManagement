using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class UserDishAssignmentService : IUserDishAssignmentService
    {
        // סטייה זניחה בחישובי double (למשל 0.1 + 0.2)
        private const double Tolerance = 0.000001;

        private readonly IMealDishRepository _mealDishes;
        private readonly IUserDishAssignmentRepository _assignments;
        private readonly IMealRepository _meals;
        private readonly IDishRepository _dishes;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserDishAssignmentService> _logger;

        public UserDishAssignmentService(
            IMealDishRepository mealDishes,
            IUserDishAssignmentRepository assignments,
            IMealRepository meals,
            IDishRepository dishes,
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserDishAssignmentService> logger)
        {
            _mealDishes = mealDishes;
            _assignments = assignments;
            _meals = meals;
            _dishes = dishes;
            _users = users;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ===== צעד 1 + 2: לקיחת מנה (כולל בחירת מאכל למנה בלתי מוגדרת) =====

        public async Task<AssignmentDto> TakeAsync(int mealId, int mealDishId, CreateAssignmentDto dto, int currentUserId, CancellationToken ct = default)
        {
            // 1. טעינה עם מעקב: ערך ה-xmin המקורי נשמר
            var mealDish = await _mealDishes.GetForUpdateAsync(mealDishId, ct);

            if (mealDish == null || mealDish.MealId != mealId)
                throw new NotFoundException("המנה לא נמצאה בארוחה הזו");

            // 2. הרשאה: בעלת הארוחה או משתתפת
            if (!await _meals.HasAccessAsync(mealId, currentUserId, ct))
                throw new ForbiddenException("רק משתתפות בארוחה יכולות לקחת מנות");

            // 3. מנה בלתי מוגדרת: קודם בוחרים מאכל
            if (mealDish.DishId == null)
                await SpecifyDishAsync(mealDish, dto, currentUserId, ct);

            // 4. בדיקה עסקית: האם נותרה מספיק כמות
            var remaining = mealDish.RequiredQuantity - mealDish.AssignedQuantity;

            if (dto.Quantity - remaining > Tolerance)
                throw new ConflictException($"נותרו רק {Math.Round(remaining, 2)} מהמנה הזו");

            // 5. עדכון בזיכרון
            mealDish.AssignedQuantity += dto.Quantity;

            var assignment = await _assignments.GetByMealDishAndUserAsync(mealDish.Id, currentUserId, ct);

            if (assignment != null)
            {
                assignment.Quantity += dto.Quantity;
            }
            else
            {
                assignment = new UserDishAssignment
                {
                    MealDish = mealDish,
                    UserId = currentUserId,
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                await _assignments.AddAsync(assignment, ct);
            }

            // 6. שמירה אחת. התנגשות על xmin ← ConflictException ← 409
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "User {UserId} took {Quantity} of MealDish {MealDishId} in meal {MealId}",
                currentUserId, dto.Quantity, mealDish.Id, mealId);

            var result = _mapper.Map<AssignmentDto>(assignment);
            var user = await _users.GetByIdAsync(currentUserId, ct);
            result.UserName = user?.FullName ?? string.Empty;
            return result;
        }

        // ===== תצוגה: מי לקחה מה =====

        public async Task<List<AssignmentDto>> GetByMealAsync(int mealId, int currentUserId, CancellationToken ct = default)
        {
            if (!await _meals.HasAccessAsync(mealId, currentUserId, ct))
            {
                if (!await _meals.ExistsAsync(mealId, ct))
                    throw new NotFoundException("הארוחה לא נמצאה");

                throw new ForbiddenException("אין הרשאה לצפות בארוחה הזו");
            }

            var assignments = await _assignments.GetByMealAsync(mealId, ct);
            return _mapper.Map<List<AssignmentDto>>(assignments);
        }

        // ===== צעד 3: ביטול לקיחה =====

        public async Task CancelAsync(int mealId, int assignmentId, int currentUserId, CancellationToken ct = default)
        {
            var assignment = await _assignments.GetForUpdateAsync(assignmentId, ct);

            if (assignment == null || assignment.MealDish.MealId != mealId)
                throw new NotFoundException("הלקיחה לא נמצאה בארוחה הזו");

            var isOwnAssignment = assignment.UserId == currentUserId;
            var isMealOwner = assignment.MealDish.Meal.OwnerId == currentUserId;

            if (!isOwnAssignment && !isMealOwner)
                throw new ForbiddenException("אפשר לבטל רק לקיחה שלך");

            // החזרת הכמות למנה. גם זה עדכון של MealDish, ולכן מוגן ב-xmin
            assignment.MealDish.AssignedQuantity =
                Math.Max(0, assignment.MealDish.AssignedQuantity - assignment.Quantity);

            _assignments.Remove(assignment);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Assignment {AssignmentId} cancelled by user {UserId}, returned {Quantity} to MealDish {MealDishId}",
                assignmentId, currentUserId, assignment.Quantity, assignment.MealDishId);
        }

        // ===== פונקציית עזר: בחירת מאכל למנה בלתי מוגדרת =====

        private async Task SpecifyDishAsync(MealDish mealDish, CreateAssignmentDto dto, int currentUserId, CancellationToken ct)
        {
            var hasExisting = dto.SpecifiedDishId.HasValue;
            var hasNew = dto.NewDish != null;

            if (hasExisting == hasNew)
                throw new BusinessRuleException("למנה בלתי מוגדרת יש לבחור מאכל מהמאגר או להוסיף מאכל חדש (אחד מהשניים)");

            Dish dish;

            if (hasExisting)
            {
                dish = await _dishes.GetByIdAsync(dto.SpecifiedDishId!.Value, ct)
                    ?? throw new NotFoundException("המאכל לא נמצא במאגר");
            }
            else
            {
                var name = dto.NewDish!.Name.Trim();

                if (await _dishes.ExistsByNameAsync(name, dto.NewDish.Category, ct))
                    throw new ConflictException($"המאכל \"{name}\" כבר קיים במאגר. יש לבחור אותו מהרשימה");

                dish = _mapper.Map<Dish>(dto.NewDish);
                dish.Name = name;
                dish.CreatedByUserId = currentUserId;
                await _dishes.AddAsync(dish, ct);
            }

            if (dish.Category != mealDish.Category)
                throw new BusinessRuleException("המאכל שנבחר אינו מתאים לסוג המנה");

            if (!KashrutRules.IsAllowed(mealDish.Meal.MealType, dish.KashrutType))
                throw new BusinessRuleException($"המאכל \"{dish.Name}\" אינו מתאים לסוג הכשרות של הארוחה");

            mealDish.Dish = dish;
            mealDish.RequiredQuantity = dish.QuantityPerPerson * mealDish.Meal.GuestsCount;
        }
    }
}
