using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MealManagement.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(value, out var id)
                ? id
                : throw new InvalidOperationException("Authenticated user has no valid 'sub' claim");
        }
    }
}