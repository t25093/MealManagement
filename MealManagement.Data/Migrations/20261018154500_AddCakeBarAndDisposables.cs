using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCakeBarAndDisposables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Category", "CreatedByUserId", "KosherType", "Name", "QuantityPerPerson", "Unit" },
                values: new object[,]
                {
                    { 3001, "CakeBar", 1, "Parve", "מגש עוגיות", 3.0, "Unit" },
                    { 3002, "CakeBar", 1, "Parve", "מאפינס", 1.0, "Unit" },
                    { 3003, "CakeBar", 1, "Parve", "רולדה", 1.0, "Portion" },
                    { 3004, "CakeBar", 1, "Parve", "עוגיות שוקולד צ'יפס", 2.0, "Unit" },
                    { 3005, "CakeBar", 1, "Parve", "כדורי שוקולד", 2.0, "Unit" },
                    { 3006, "CakeBar", 1, "Parve", "עוגת גזר", 1.0, "Portion" },
                    { 3007, "CakeBar", 1, "Parve", "טארט פירות", 1.0, "Portion" },
                    { 3008, "CakeBar", 1, "Parve", "פחזניות", 2.0, "Unit" },
                    { 3009, "CakeBar", 1, "Dairy", "עוגת גבינה אפויה", 1.0, "Portion" },
                    { 3010, "CakeBar", 1, "Dairy", "טירמיסו", 1.0, "Portion" },
                    { 3021, "Disposables", 1, "Parve", "צלחות חד פעמיות גדולות", 1.0, "Unit" },
                    { 3022, "Disposables", 1, "Parve", "צלחות חד פעמיות קטנות", 2.0, "Unit" },
                    { 3023, "Disposables", 1, "Parve", "כוסות חד פעמיות", 3.0, "Unit" },
                    { 3024, "Disposables", 1, "Parve", "סט סכין, מזלג וכף", 1.0, "Unit" },
                    { 3025, "Disposables", 1, "Parve", "מפיות", 3.0, "Unit" },
                    { 3026, "Disposables", 1, "Parve", "קערות חד פעמיות", 1.0, "Unit" },
                    { 3027, "Disposables", 1, "Parve", "מפות שולחן חד פעמיות", 0.10000000000000001, "Unit" },
                    { 3028, "Disposables", 1, "Parve", "מגשי הגשה", 0.10000000000000001, "Unit" }
                });

            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Dishes""', 'Id'), (SELECT MAX(""Id"") FROM ""Dishes""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3001);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3002);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3003);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3004);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3005);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3006);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3007);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3008);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3009);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3010);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3021);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3022);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3023);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3024);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3025);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3026);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3027);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3028);
        }
    }
}
