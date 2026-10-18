using Npgsql;

namespace MealManagement.API.Extensions
{
    public static class ConnectionStringHelper
    {
        // Render נותן כתובת בפורמט postgresql://user:pass@host:port/db.
        // Npgsql מצפה לפורמט Host=...;Username=..., ולכן ממירים. מחרוזת רגילה חוזרת כמו שהיא.
        public static string ToNpgsql(string connectionString)
        {
            if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
                !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                return connectionString;
            }

            var uri = new Uri(connectionString);
            var userInfo = uri.UserInfo.Split(':', 2);

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.IsDefaultPort || uri.Port <= 0 ? 5432 : uri.Port,
                Database = uri.AbsolutePath.TrimStart('/'),
                Username = Uri.UnescapeDataString(userInfo[0]),
                Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null
            };

            // כתובת חיצונית של Render (עם נקודה בשם השרת) דורשת SSL
            if (uri.Host.Contains('.'))
            {
                builder.SslMode = SslMode.Require;
            }

            return builder.ConnectionString;
        }
    }
}
