using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class MealListItemService : IMealListItemService
    {
        private readonly IMealListItemRepository _items;
        private readonly IMealRepository _meals;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MealListItemService> _logger;

        public MealListItemService(
            IMealListItemRepository items,
            IMealRepository meals,
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MealListItemService> logger)
        {
            _items = items;
            _meals = meals;
            _users = users;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<MealListItemDto>> GetByMealAsync(int mealId, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            var items = await _items.GetByMealAsync(mealId, ct);
            return _mapper.Map<List<MealListItemDto>>(items);
        }

        public async Task<MealListItemDto> CreateAsync(int mealId, CreateMealListItemDto dto, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);

            var name = dto.Name?.Trim();
            if (string.IsNullOrEmpty(name))
                throw new BusinessRuleException("יש להזין שם");

            var item = new MealListItem
            {
                MealId = mealId,
                ListType = dto.ListType!.Value,
                Name = name,
                Note = CleanNote(dto.Note),
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow,
            };

            await _items.AddAsync(item, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("List item {ItemId} ({ListType}) added to meal {MealId} by user {UserId}",
                item.Id, item.ListType, mealId, currentUserId);

            item.CreatedBy = await _users.GetByIdAsync(currentUserId, ct);
            return _mapper.Map<MealListItemDto>(item);
        }

        public async Task<MealListItemDto> UpdateAsync(int mealId, int itemId, UpdateMealListItemDto dto, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);
            var item = await GetItemAsync(mealId, itemId, ct);

            if (dto.Name != null)
            {
                var name = dto.Name.Trim();
                if (name.Length == 0)
                    throw new BusinessRuleException("יש להזין שם");
                item.Name = name;
            }

            // null: ההערה לא נשלחה ולא משתנה. מחרוזת ריקה: מוחקת את ההערה.
            if (dto.Note != null)
                item.Note = CleanNote(dto.Note);

            if (dto.IsDone.HasValue && dto.IsDone.Value != item.IsDone)
            {
                item.IsDone = dto.IsDone.Value;
                if (item.IsDone)
                {
                    item.DoneByUserId = currentUserId;
                    item.DoneBy = await _users.GetByIdAsync(currentUserId, ct);
                    item.DoneAt = DateTime.UtcNow;
                }
                else
                {
                    item.DoneByUserId = null;
                    item.DoneBy = null;
                    item.DoneAt = null;
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return _mapper.Map<MealListItemDto>(item);
        }

        public async Task DeleteAsync(int mealId, int itemId, int currentUserId, CancellationToken ct = default)
        {
            await EnsureAccessAsync(mealId, currentUserId, ct);
            var item = await GetItemAsync(mealId, itemId, ct);

            _items.Remove(item);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("List item {ItemId} deleted from meal {MealId} by user {UserId}",
                itemId, mealId, currentUserId);
        }

        private async Task<MealListItem> GetItemAsync(int mealId, int itemId, CancellationToken ct)
        {
            var item = await _items.GetByIdAsync(itemId, ct);
            if (item == null || item.MealId != mealId)
                throw new NotFoundException("הפריט לא נמצא ברשימות של הארוחה הזו");
            return item;
        }

        private static string? CleanNote(string? note)
        {
            var trimmed = note?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }

        private async Task EnsureAccessAsync(int mealId, int currentUserId, CancellationToken ct)
        {
            if (await _meals.HasAccessAsync(mealId, currentUserId, ct))
                return;

            if (!await _meals.ExistsAsync(mealId, ct))
                throw new NotFoundException("הארוחה לא נמצאה");

            throw new ForbiddenException("אין הרשאה לארוחה הזו");
        }
    }
}
