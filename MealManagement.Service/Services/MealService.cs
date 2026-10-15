using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _meals;
        private readonly IDishRepository _dishes;
        private readonly IUserRepository _users;
        private readonly IUserDishAssignmentRepository _assignments;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MealService> _logger;

        public MealService(
            IMealRepository meals,
            IDishRepository dishes,
            IUserRepository users,
            IUserDishAssignmentRepository assignments,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MealService> logger)
        {
            _meals = meals;
            _dishes = dishes;
            _users = users;
            _assignments = assignments;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ===== צעד 1: יצירת ארוחה =====

        public async Task<MealDetailsDto> CreateAsync(CreateMealDto dto, int currentUserId, CancellationToken ct = default)
        {
            // 1. שליפת כל המאכלים שנבחרו, בשאילתה אחת
            var dishIds = dto.MenuItems
                .Where(i => i.DishId.HasValue)
                .Select(i => i.DishId!.Value)
                .Distinct()
                .ToList();

            var dishes = await _dishes.GetByIdsAsync(dishIds, ct);
            var dishById = dishes.ToDictionary(d => d.Id);

            // 2. מאכל שלא קיים
            var missingIds = dishIds.Where(id => !dishById.ContainsKey(id)).ToList();
            if (missingIds.Count > 0)
                throw new NotFoundException($"מאכלים שלא נמצאו במאגר: {string.Join(", ", missingIds)}");

            // 3. ולידציית כשרות
            var notAllowed = dishes
                .Where(d => !KashrutRules.IsAllowed(dto.MealType, d.KosherType))
                .Select(d => d.Name)
                .ToList();
            if (notAllowed.Count > 0)
                throw new BusinessRuleException($"המאכלים הבאים אינם מתאימים לסוג הכשרות של הארוחה: {string.Join(", ", notAllowed)}");

            // 4. בניית הארוחה
            var meal = _mapper.Map<Meal>(dto);
            meal.OwnerId = currentUserId;
            meal.EventDate = ToUtc(dto.EventDate);
            meal.CreatedAt = DateTime.UtcNow;

            // 5. בניית המנות וחישוב הכמויות
            foreach (var item in dto.MenuItems)
            {
                if (item.DishId.HasValue)
                {
                    var dish = dishById[item.DishId.Value];
                    meal.MealDishes.Add(new MealDish
                    {
                        DishId = dish.Id,
                        Category = dish.Category,
                        RequiredQuantity = dish.QuantityPerPerson * meal.GuestsCount,
                        AssignedQuantity = 0
                    });
                }
                else
                {
                    meal.MealDishes.Add(new MealDish
                    {
                        DishId = null,
                        Category = item.Category,
                        RequiredQuantity = 0,
                        AssignedQuantity = 0
                    });
                }
            }

            // 6. שמירה
            await _meals.AddAsync(meal, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Meal {MealId} created by user {UserId} with {DishCount} dishes",
                meal.Id, currentUserId, meal.MealDishes.Count);

            return await LoadDetailsAsync(meal.Id, currentUserId, ct);
        }

        // ===== צעד 2: רשימות ופרטי ארוחה =====

        public async Task<PagedResult<MealSummaryDto>> GetMyMealsAsync(MealQueryParams query, int currentUserId, CancellationToken ct = default)
        {
            query.Page = Math.Max(query.Page, 1);

            var result = await _meals.GetPagedForUserAsync(currentUserId, query, ct);

            var items = result.Items
                .Select(meal =>
                {
                    var dto = _mapper.Map<MealSummaryDto>(meal);
                    dto.CoveragePercent = CalculateCoverage(meal);
                    return dto;
                })
                .ToList();

            return new PagedResult<MealSummaryDto>
            {
                Items = items,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<MealDetailsDto> GetByIdAsync(int mealId, int currentUserId, CancellationToken ct = default)
        {
            return await LoadDetailsAsync(mealId, currentUserId, ct);
        }

        // ===== צעד 3: עדכון שם ומספר אורחים =====

        public async Task UpdateAsync(int mealId, UpdateMealDto dto, int currentUserId, CancellationToken ct = default)
        {
            var meal = await GetMealForUpdateOrThrowAsync(mealId, ct);
            EnsureOwner(meal, currentUserId);

            if (!string.IsNullOrWhiteSpace(dto.Name))
                meal.Name = dto.Name.Trim();

            if (dto.GuestsCount.HasValue && dto.GuestsCount.Value != meal.GuestsCount)
            {
                var newGuestsCount = dto.GuestsCount.Value;

                var dishIds = meal.MealDishes
                    .Where(md => md.DishId.HasValue)
                    .Select(md => md.DishId!.Value)
                    .Distinct()
                    .ToList();

                var dishById = (await _dishes.GetByIdsAsync(dishIds, ct)).ToDictionary(d => d.Id);

                var tooLow = new List<string>();

                foreach (var mealDish in meal.MealDishes.Where(md => md.DishId.HasValue))
                {
                    var dish = dishById[mealDish.DishId!.Value];
                    var newRequired = dish.QuantityPerPerson * newGuestsCount;

                    if (newRequired < mealDish.AssignedQuantity)
                        tooLow.Add(dish.Name);
                    else
                        mealDish.RequiredQuantity = newRequired;
                }

                if (tooLow.Count > 0)
                    throw new ConflictException(
                        $"לא ניתן להוריד את מספר האורחים: מהמנות הבאות כבר נלקחה כמות גדולה יותר: {string.Join(", ", tooLow)}");

                meal.GuestsCount = newGuestsCount;
            }

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Meal {MealId} updated by user {UserId}", mealId, currentUserId);
        }

        // ===== צעד 4: מחיקה ומשתתפות =====

        public async Task DeleteAsync(int mealId, int currentUserId, bool isAdmin, CancellationToken ct = default)
        {
            var meal = await GetMealForUpdateOrThrowAsync(mealId, ct);

            if (!isAdmin)
                EnsureOwner(meal, currentUserId);

            _meals.Remove(meal);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Meal {MealId} deleted by user {UserId}", mealId, currentUserId);
        }

        public async Task<UserDto> AddParticipantAsync(int mealId, AddParticipantDto dto, int currentUserId, CancellationToken ct = default)
        {
            var meal = await GetMealForUpdateOrThrowAsync(mealId, ct);
            EnsureOwner(meal, currentUserId);

            var email = dto.Email.Trim().ToLowerInvariant();

            var found = await _users.GetByEmailAsync(email, ct)
                ?? throw new NotFoundException($"לא נמצאה משתמשת רשומה עם האימייל {email}");

            if (found.Id == meal.OwnerId)
                throw new BusinessRuleException("בעלת הארוחה כבר חלק מהארוחה");

            if (meal.Participants.Any(p => p.Id == found.Id))
                throw new ConflictException("המשתמשת כבר משתתפת בארוחה");

            // טעינה מחדש עם מעקב, כדי ש-EF יוסיף רק שורה לטבלת הקשר
            var participant = await _users.GetByIdAsync(found.Id, ct)
                ?? throw new NotFoundException("המשתמשת לא נמצאה");

            meal.Participants.Add(participant);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("User {ParticipantId} added to meal {MealId}", participant.Id, mealId);

            return _mapper.Map<UserDto>(participant);
        }

        public async Task RemoveParticipantAsync(int mealId, int participantId, int currentUserId, CancellationToken ct = default)
        {
            var meal = await GetMealForUpdateOrThrowAsync(mealId, ct);

            // בעלת הארוחה מסירה כל משתתפת, ומשתתפת יכולה להסיר את עצמה
            if (meal.OwnerId != currentUserId && participantId != currentUserId)
                throw new ForbiddenException("רק בעלת הארוחה יכולה להסיר משתתפות");

            var participant = meal.Participants.FirstOrDefault(p => p.Id == participantId)
                ?? throw new NotFoundException("המשתמשת אינה משתתפת בארוחה");

            var assignments = await _assignments.GetByMealAsync(mealId, ct);
            if (assignments.Any(a => a.UserId == participantId))
                throw new ConflictException("למשתתפת יש מנות שלקחה בארוחה. יש לבטל אותן לפני ההסרה");

            meal.Participants.Remove(participant);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("User {ParticipantId} removed from meal {MealId}", participantId, mealId);
        }

        // ===== פונקציות עזר =====

        private async Task<MealDetailsDto> LoadDetailsAsync(int mealId, int currentUserId, CancellationToken ct)
        {
            var meal = await _meals.GetDetailsAsync(mealId, ct)
                ?? throw new NotFoundException("הארוחה לא נמצאה");

            var isOwner = meal.OwnerId == currentUserId;
            var isParticipant = meal.Participants.Any(p => p.Id == currentUserId);

            if (!isOwner && !isParticipant)
                throw new ForbiddenException("אין הרשאה לצפות בארוחה הזו");

            var dto = _mapper.Map<MealDetailsDto>(meal);
            dto.IsOwner = isOwner;
            return dto;
        }

        private async Task<Meal> GetMealForUpdateOrThrowAsync(int mealId, CancellationToken ct)
        {
            return await _meals.GetForUpdateAsync(mealId, ct)
                ?? throw new NotFoundException("הארוחה לא נמצאה");
        }

        private static void EnsureOwner(Meal meal, int currentUserId)
        {
            if (meal.OwnerId != currentUserId)
                throw new ForbiddenException("רק בעלת הארוחה יכולה לבצע פעולה זו");
        }

        private static double CalculateCoverage(Meal meal)
        {
            var total = meal.MealDishes.Count;
            if (total == 0)
                return 0;

            var covered = meal.MealDishes.Count(md =>
                md.DishId != null && md.AssignedQuantity >= md.RequiredQuantity);

            return Math.Round(covered * 100.0 / total, 1);
        }

        private static DateTime ToUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }
    }
}
