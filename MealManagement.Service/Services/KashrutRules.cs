using MealManagement.Core.Enums;

namespace MealManagement.Service.Services
{
    public static class KashrutRules
    {
        public static bool IsAllowed(MealType mealType, MealType dishKashrut)
        {
            return dishKashrut == MealType.Parve || dishKashrut == mealType;
        }
    }
}