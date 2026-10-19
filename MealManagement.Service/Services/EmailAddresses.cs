using System.Text.RegularExpressions;

namespace MealManagement.Service.Services
{
    // אימייל אחיד בכל המערכת: בלי רווחים (גם באמצע), בלי סימני כיווניות נסתרים שנדבקים בהעתקה, ובאותיות קטנות
    public static class EmailAddresses
    {
        private static readonly Regex SpacesAndInvisible =
            new(@"[\s​-‏‪-‮⁦-⁩﻿]", RegexOptions.Compiled);

        public static string Normalize(string email)
        {
            return SpacesAndInvisible.Replace(email, string.Empty).ToLowerInvariant();
        }
    }
}
