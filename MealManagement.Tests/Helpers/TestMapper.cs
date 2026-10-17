using AutoMapper;
using MealManagement.Service.Mapping;

namespace MealManagement.Tests.Helpers
{
    // AutoMapper אמיתי עם ה-Profile של הפרויקט (לא מדומה)
    public static class TestMapper
    {
        public static IMapper Create()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }
    }
}
