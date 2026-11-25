using MealManagement.Service.Services;

namespace MealManagement.Tests.Services
{
    public class ChatTypingStoreTests
    {
        // שעון שאפשר להזיז בבדיקה
        private sealed class ManualTime : TimeProvider
        {
            public DateTimeOffset Now { get; set; } = new(2026, 11, 25, 12, 0, 0, TimeSpan.Zero);
            public override DateTimeOffset GetUtcNow() => Now;
        }

        private readonly ManualTime _time = new();
        private readonly ChatTypingStore _store;

        public ChatTypingStoreTests()
        {
            _store = new ChatTypingStore(_time);
        }

        [Fact]
        public void GetActive_ReturnsOthersWithTextAndNotMyself()
        {
            _store.Set(mealId: 1, userId: 2, "רחל לוי", "כן, צריך עוד");
            _store.Set(mealId: 1, userId: 3, "שרה כהן", "אני");

            var active = _store.GetActive(mealId: 1, exceptUserId: 3);

            var typing = Assert.Single(active);
            Assert.Equal(2, typing.UserId);
            Assert.Equal("רחל לוי", typing.Name);
            Assert.Equal("כן, צריך עוד", typing.Text);
        }

        [Fact]
        public void GetActive_OtherMeal_IsNotShown()
        {
            _store.Set(mealId: 1, userId: 2, "רחל", "שלום");

            Assert.Empty(_store.GetActive(mealId: 2, exceptUserId: 9));
        }

        [Fact]
        public void Set_SameUserAgain_ReplacesTheText()
        {
            _store.Set(1, 2, "רחל", "של");
            _store.Set(1, 2, "רחל", "שלום");

            Assert.Equal("שלום", Assert.Single(_store.GetActive(1, 9)).Text);
        }

        [Fact]
        public void Clear_RemovesTheEntry()
        {
            _store.Set(1, 2, "רחל", "שלום");

            _store.Clear(1, 2);

            Assert.Empty(_store.GetActive(1, 9));
        }

        [Fact]
        public void GetActive_EntryNotUpdatedForLifetime_IsGone()
        {
            _store.Set(1, 2, "רחל", "שלום");

            _time.Now += ChatTypingStore.Lifetime - TimeSpan.FromSeconds(1);
            Assert.Single(_store.GetActive(1, 9));

            _time.Now += TimeSpan.FromSeconds(2);
            Assert.Empty(_store.GetActive(1, 9));
        }
    }
}
