using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Service.Services;
using MealManagement.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MealManagement.Tests.Services
{
    public class MealListItemServiceTests
    {
        private const int MealId = 7;
        private const int UserId = 2;

        private readonly Mock<IMealListItemRepository> _items = new();
        private readonly Mock<IMealRepository> _meals = new();
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private readonly MealListItemService _service;

        public MealListItemServiceTests()
        {
            _service = new MealListItemService(
                _items.Object,
                _meals.Object,
                _users.Object,
                _unitOfWork.Object,
                TestMapper.Create(),
                NullLogger<MealListItemService>.Instance);

            _meals.Setup(m => m.HasAccessAsync(MealId, UserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _users.Setup(u => u.GetByIdAsync(UserId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new User { Id = UserId, FullName = "רחל לוי" });
        }

        [Fact]
        public async Task CreateAsync_TrimsNameAndEmptyNote()
        {
            MealListItem? added = null;
            _items.Setup(i => i.AddAsync(It.IsAny<MealListItem>(), It.IsAny<CancellationToken>()))
                  .Callback<MealListItem, CancellationToken>((item, _) => added = item);

            var result = await _service.CreateAsync(MealId,
                new CreateMealListItemDto { ListType = MealListType.Shopping, Name = "  חלות  ", Note = "   " }, UserId);

            Assert.NotNull(added);
            Assert.Equal("חלות", added!.Name);
            Assert.Null(added.Note);
            Assert.Equal(MealListType.Shopping, added.ListType);
            Assert.Equal(UserId, added.CreatedByUserId);
            Assert.False(added.IsDone);
            Assert.Equal("רחל לוי", result.CreatedByName);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_NoAccess_ThrowsForbidden()
        {
            _meals.Setup(m => m.HasAccessAsync(MealId, 99, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _meals.Setup(m => m.ExistsAsync(MealId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.CreateAsync(MealId,
                new CreateMealListItemDto { ListType = MealListType.Bring, Name = "מיחם" }, 99));
        }

        [Fact]
        public async Task UpdateAsync_MarkDone_SavesWhoAndWhen()
        {
            var item = new MealListItem { Id = 5, MealId = MealId, Name = "מיחם", ListType = MealListType.Bring };
            _items.Setup(i => i.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(item);

            var result = await _service.UpdateAsync(MealId, 5, new UpdateMealListItemDto { IsDone = true }, UserId);

            Assert.True(item.IsDone);
            Assert.Equal(UserId, item.DoneByUserId);
            Assert.NotNull(item.DoneAt);
            Assert.Equal("רחל לוי", result.DoneByName);
        }

        [Fact]
        public async Task UpdateAsync_Unmark_ClearsWho()
        {
            var item = new MealListItem { Id = 5, MealId = MealId, Name = "מיחם", IsDone = true, DoneByUserId = 3, DoneAt = DateTime.UtcNow };
            _items.Setup(i => i.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(item);

            await _service.UpdateAsync(MealId, 5, new UpdateMealListItemDto { IsDone = false }, UserId);

            Assert.False(item.IsDone);
            Assert.Null(item.DoneByUserId);
            Assert.Null(item.DoneAt);
        }

        [Fact]
        public async Task UpdateAsync_EmptyNoteClears_NullNoteKeeps()
        {
            var item = new MealListItem { Id = 5, MealId = MealId, Name = "יין", Note = "2 בקבוקים" };
            _items.Setup(i => i.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(item);

            await _service.UpdateAsync(MealId, 5, new UpdateMealListItemDto { Name = "יין לקידוש" }, UserId);
            Assert.Equal("יין לקידוש", item.Name);
            Assert.Equal("2 בקבוקים", item.Note);

            await _service.UpdateAsync(MealId, 5, new UpdateMealListItemDto { Note = "" }, UserId);
            Assert.Null(item.Note);
        }

        [Fact]
        public async Task UpdateAsync_ItemOfOtherMeal_ThrowsNotFound()
        {
            _items.Setup(i => i.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new MealListItem { Id = 5, MealId = 99, Name = "x" });

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdateAsync(MealId, 5, new UpdateMealListItemDto { IsDone = true }, UserId));
        }

        [Fact]
        public async Task DeleteAsync_RemovesItem()
        {
            var item = new MealListItem { Id = 5, MealId = MealId, Name = "מפיות" };
            _items.Setup(i => i.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(item);

            await _service.DeleteAsync(MealId, 5, UserId);

            _items.Verify(i => i.Remove(item), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
