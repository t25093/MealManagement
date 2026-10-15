using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateAt", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@meals.local", "מנהלת המערכת", "$2b$11$XuY3CT.E4JB3LVUquipoXeaoDF8voHOA1hx6nBShMB6tZQfD.etxm", "Admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "user@meals.local", "משתמשת לדוגמה", "$2b$11$bQ5cSLxZkd1tjqGEsgqKl.r88vIa6nBg2kTWoEbP.gLo8.v3UTCpa", "User" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Category", "CreatedByUserId", "KosherType", "Name", "QuantityPerPerson", "Unit" },
                values: new object[,]
                {
                    { 1, "Starter", 1, "Parve", "חומוס", 0.10000000000000001, "Kilogram" },
                    { 2, "Starter", 1, "Parve", "טחינה", 0.050000000000000003, "Kilogram" },
                    { 3, "Starter", 1, "Parve", "גפילטע פיש", 1.0, "Unit" },
                    { 4, "Salad", 1, "Parve", "סלט ירקות", 0.14999999999999999, "Kilogram" },
                    { 5, "Salad", 1, "Parve", "סלט כרוב", 0.10000000000000001, "Kilogram" },
                    { 6, "Salad", 1, "Parve", "סלט חצילים", 0.080000000000000002, "Kilogram" },
                    { 7, "Main", 1, "Meat", "עוף צלוי", 0.29999999999999999, "Kilogram" },
                    { 8, "Main", 1, "Meat", "צלי בקר", 0.20000000000000001, "Kilogram" },
                    { 9, "Main", 1, "Meat", "קציצות בשר", 3.0, "Unit" },
                    { 10, "Main", 1, "Meat", "שניצל", 2.0, "Unit" },
                    { 11, "Main", 1, "Dairy", "לזניה", 1.0, "Portion" },
                    { 12, "Main", 1, "Dairy", "קיש בצל", 1.0, "Portion" },
                    { 13, "Side", 1, "Parve", "אורז", 0.10000000000000001, "Kilogram" },
                    { 14, "Side", 1, "Parve", "קוגל תפוחי אדמה", 1.0, "Portion" },
                    { 15, "Side", 1, "Parve", "קוגל ירושלמי", 1.0, "Portion" },
                    { 16, "Side", 1, "Parve", "תפוחי אדמה אפויים", 0.20000000000000001, "Kilogram" },
                    { 17, "Dessert", 1, "Parve", "עוגת שוקולד", 1.0, "Portion" },
                    { 18, "Dessert", 1, "Dairy", "עוגת גבינה", 1.0, "Portion" },
                    { 19, "Dessert", 1, "Parve", "סלט פירות", 0.14999999999999999, "Kilogram" },
                    { 20, "Drink", 1, "Parve", "מיץ ענבים", 0.25, "Liter" }
                });

            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "Id", "CreatedAt", "EventDate", "GuestsCount", "MealType", "Name", "OwnerId" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 1, 17, 0, 0, 0, DateTimeKind.Utc), 30, "Meat", "ארוחת ערב משפחתית", 1 });

            migrationBuilder.InsertData(
                table: "MealParticipants",
                columns: new[] { "ParticipantsId", "ParticipatingMealsId" },
                values: new object[] { 2, 1 });

            // קידום המונים אחרי הכנסת מזהים קבועים ב-Seed
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Users""', 'Id'), (SELECT MAX(""Id"") FROM ""Users""));");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Dishes""', 'Id'), (SELECT MAX(""Id"") FROM ""Dishes""));");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Meals""', 'Id'), (SELECT MAX(""Id"") FROM ""Meals""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MealParticipants",
                keyColumns: new[] { "ParticipantsId", "ParticipatingMealsId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
