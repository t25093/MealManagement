using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreDishes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Category", "CreatedByUserId", "KosherType", "Name", "QuantityPerPerson", "Unit" },
                values: new object[,]
                {
                    { 1001, "Starter", 1, "Meat", "כבד קצוץ", 0.050000000000000003, "Kilogram" },
                    { 1002, "Starter", 1, "Meat", "מרק עוף", 1.0, "Portion" },
                    { 1003, "Salad", 1, "Meat", "סלט עוף", 0.10000000000000001, "Kilogram" },
                    { 1004, "Main", 1, "Meat", "שוקיים בתנור", 1.0, "Unit" },
                    { 1005, "Main", 1, "Meat", "גולאש", 0.25, "Kilogram" },
                    { 1006, "Side", 1, "Meat", "חמין", 0.25, "Kilogram" },
                    { 1007, "Side", 1, "Meat", "אורז עם בשר טחון", 0.14999999999999999, "Kilogram" },
                    { 1011, "Starter", 1, "Dairy", "בורקס גבינה", 2.0, "Unit" },
                    { 1012, "Salad", 1, "Dairy", "סלט יווני", 0.14999999999999999, "Kilogram" },
                    { 1013, "Main", 1, "Dairy", "פשטידת ירקות", 1.0, "Portion" },
                    { 1014, "Main", 1, "Dairy", "פסטה ברוטב שמנת", 1.0, "Portion" },
                    { 1015, "Main", 1, "Dairy", "בלינצ'ס גבינה", 2.0, "Unit" },
                    { 1016, "Side", 1, "Dairy", "תפוחי אדמה מוקרמים", 1.0, "Portion" },
                    { 1017, "Dessert", 1, "Dairy", "מוס שוקולד", 1.0, "Portion" },
                    { 1018, "Drink", 1, "Dairy", "קפה קר", 0.25, "Liter" },
                    { 1021, "Starter", 1, "Parve", "מטבוחה", 0.050000000000000003, "Kilogram" },
                    { 1022, "Salad", 1, "Parve", "סלט חסה", 0.10000000000000001, "Kilogram" },
                    { 1023, "Salad", 1, "Parve", "סלט תפוחי אדמה", 0.12, "Kilogram" },
                    { 1024, "Side", 1, "Parve", "ירקות מוקפצים", 0.14999999999999999, "Kilogram" },
                    { 1025, "Side", 1, "Parve", "קוגל אטריות", 1.0, "Portion" },
                    { 1026, "Side", 1, "Parve", "שעועית ירוקה", 0.10000000000000001, "Kilogram" },
                    { 1027, "Dessert", 1, "Parve", "עוגיות", 3.0, "Unit" },
                    { 1028, "Dessert", 1, "Parve", "סורבה", 1.0, "Portion" },
                    { 1029, "Drink", 1, "Parve", "מים מינרליים", 0.5, "Liter" },
                    { 1030, "Drink", 1, "Parve", "מיץ תפוזים", 0.25, "Liter" }
                });
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Dishes""', 'Id'), (SELECT MAX(""Id"") FROM ""Dishes""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1028);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1029);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1030);
        }
    }
}
