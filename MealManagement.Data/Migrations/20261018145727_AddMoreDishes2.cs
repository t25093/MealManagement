using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreDishes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Category", "CreatedByUserId", "KosherType", "Name", "QuantityPerPerson", "Unit" },
                values: new object[,]
                {
                    { 2001, "Starter", 1, "Meat", "סיגרים בבשר", 2.0, "Unit" },
                    { 2002, "Starter", 1, "Meat", "קובה", 2.0, "Unit" },
                    { 2003, "Starter", 1, "Meat", "כנפיים ברוטב", 3.0, "Unit" },
                    { 2004, "Starter", 1, "Meat", "מרק בקר וירקות", 1.0, "Portion" },
                    { 2005, "Salad", 1, "Meat", "סלט פסטרמה", 0.10000000000000001, "Kilogram" },
                    { 2006, "Salad", 1, "Meat", "סלט עוף אסייתי", 0.10000000000000001, "Kilogram" },
                    { 2007, "Main", 1, "Meat", "פשטידת בשר", 1.0, "Portion" },
                    { 2008, "Main", 1, "Meat", "שניצל עוף אפוי", 2.0, "Unit" },
                    { 2009, "Main", 1, "Meat", "חזה עוף בגריל", 1.0, "Unit" },
                    { 2010, "Main", 1, "Meat", "פרגיות על האש", 0.25, "Kilogram" },
                    { 2011, "Main", 1, "Meat", "צלי הודו", 0.20000000000000001, "Kilogram" },
                    { 2012, "Main", 1, "Meat", "בשר בקר בתנור", 0.20000000000000001, "Kilogram" },
                    { 2013, "Main", 1, "Meat", "קציצות עוף ברוטב", 3.0, "Unit" },
                    { 2014, "Main", 1, "Meat", "עוף בתפוזים", 0.29999999999999999, "Kilogram" },
                    { 2015, "Main", 1, "Meat", "ממולאים בבשר", 2.0, "Unit" },
                    { 2016, "Main", 1, "Meat", "לשון ברוטב", 0.14999999999999999, "Kilogram" },
                    { 2017, "Main", 1, "Meat", "נקניקיות", 2.0, "Unit" },
                    { 2018, "Main", 1, "Meat", "פסטה בולונז", 1.0, "Portion" },
                    { 2019, "Main", 1, "Meat", "מוסקה בשרית", 1.0, "Portion" },
                    { 2020, "Side", 1, "Meat", "פתיתים עם עוף", 0.14999999999999999, "Kilogram" },
                    { 2021, "Side", 1, "Meat", "קישקע", 1.0, "Portion" },
                    { 2022, "Side", 1, "Meat", "שעועית עם בשר", 0.14999999999999999, "Kilogram" },
                    { 2023, "Side", 1, "Meat", "קוסקוס עם מרק עוף", 0.20000000000000001, "Kilogram" },
                    { 2031, "Starter", 1, "Dairy", "מרק שמנת פטריות", 1.0, "Portion" },
                    { 2032, "Starter", 1, "Dairy", "מגש גבינות", 0.080000000000000002, "Kilogram" },
                    { 2033, "Starter", 1, "Dairy", "טוסטים קטנים", 2.0, "Unit" },
                    { 2034, "Starter", 1, "Dairy", "מרק בטטה בשמנת", 1.0, "Portion" },
                    { 2035, "Salad", 1, "Dairy", "סלט קפרזה", 0.12, "Kilogram" },
                    { 2036, "Salad", 1, "Dairy", "סלט קיסר", 0.12, "Kilogram" },
                    { 2037, "Salad", 1, "Dairy", "סלט פסטה עם גבינה", 0.12, "Kilogram" },
                    { 2038, "Main", 1, "Dairy", "פיצה", 2.0, "Unit" },
                    { 2039, "Main", 1, "Dairy", "רביולי גבינה", 1.0, "Portion" },
                    { 2040, "Main", 1, "Dairy", "פסטה ברוטב רוזה", 1.0, "Portion" },
                    { 2041, "Main", 1, "Dairy", "קיש פטריות", 1.0, "Portion" },
                    { 2042, "Main", 1, "Dairy", "פשטידת ברוקולי", 1.0, "Portion" },
                    { 2043, "Main", 1, "Dairy", "שקשוקה עם גבינה", 1.0, "Portion" },
                    { 2044, "Main", 1, "Dairy", "ניוקי ברוטב שמנת", 1.0, "Portion" },
                    { 2045, "Side", 1, "Dairy", "לחמניות גבינה", 2.0, "Unit" },
                    { 2046, "Side", 1, "Dairy", "פירה בחמאה", 0.14999999999999999, "Kilogram" },
                    { 2047, "Side", 1, "Dairy", "אורז מוקרם", 0.12, "Kilogram" },
                    { 2048, "Side", 1, "Dairy", "ירקות מוקרמים", 0.14999999999999999, "Kilogram" },
                    { 2049, "Dessert", 1, "Dairy", "עוגת גבינה פירורים", 1.0, "Portion" },
                    { 2050, "Dessert", 1, "Dairy", "גלידה", 1.0, "Portion" },
                    { 2051, "Dessert", 1, "Dairy", "מלבי", 1.0, "Portion" },
                    { 2052, "Dessert", 1, "Dairy", "עוגת שמרים", 1.0, "Portion" },
                    { 2053, "Dessert", 1, "Dairy", "פנקייקים", 2.0, "Unit" },
                    { 2054, "Drink", 1, "Dairy", "שוקו", 0.25, "Liter" },
                    { 2055, "Drink", 1, "Dairy", "מילקשייק", 0.25, "Liter" },
                    { 2056, "Drink", 1, "Dairy", "קפה הפוך", 0.20000000000000001, "Liter" },
                    { 2061, "Starter", 1, "Parve", "פלאפל", 3.0, "Unit" },
                    { 2062, "Starter", 1, "Parve", "חציל בטחינה", 0.080000000000000002, "Kilogram" },
                    { 2063, "Starter", 1, "Parve", "ממרח פלפלים קלויים", 0.050000000000000003, "Kilogram" },
                    { 2064, "Starter", 1, "Parve", "זיתים", 0.040000000000000001, "Kilogram" },
                    { 2065, "Starter", 1, "Parve", "מרק ירקות", 1.0, "Portion" },
                    { 2066, "Starter", 1, "Parve", "מרק עדשים", 1.0, "Portion" },
                    { 2067, "Salad", 1, "Parve", "סלט טבולה", 0.10000000000000001, "Kilogram" },
                    { 2068, "Salad", 1, "Parve", "סלט גזר", 0.080000000000000002, "Kilogram" },
                    { 2069, "Salad", 1, "Parve", "סלט סלק", 0.080000000000000002, "Kilogram" },
                    { 2070, "Salad", 1, "Parve", "סלט קינואה", 0.10000000000000001, "Kilogram" },
                    { 2071, "Salad", 1, "Parve", "סלט פסטה", 0.12, "Kilogram" },
                    { 2072, "Salad", 1, "Parve", "סלט כרוב סגול", 0.080000000000000002, "Kilogram" },
                    { 2073, "Salad", 1, "Parve", "סלט מלפפונים", 0.080000000000000002, "Kilogram" },
                    { 2074, "Main", 1, "Parve", "שקשוקה", 1.0, "Portion" },
                    { 2075, "Main", 1, "Parve", "פשטידת תרד", 1.0, "Portion" },
                    { 2076, "Main", 1, "Parve", "מג'דרה", 0.20000000000000001, "Kilogram" },
                    { 2077, "Main", 1, "Parve", "ממולאים באורז", 2.0, "Unit" },
                    { 2078, "Main", 1, "Parve", "קציצות ירק", 3.0, "Unit" },
                    { 2079, "Main", 1, "Parve", "פסטה ברוטב עגבניות", 1.0, "Portion" },
                    { 2080, "Side", 1, "Parve", "חלה", 1.0, "Portion" },
                    { 2081, "Side", 1, "Parve", "פתיתים", 0.12, "Kilogram" },
                    { 2082, "Side", 1, "Parve", "קוסקוס", 0.14999999999999999, "Kilogram" },
                    { 2083, "Side", 1, "Parve", "בטטה בתנור", 0.14999999999999999, "Kilogram" },
                    { 2084, "Side", 1, "Parve", "תירס", 1.0, "Unit" },
                    { 2085, "Side", 1, "Parve", "פירה", 0.14999999999999999, "Kilogram" },
                    { 2086, "Side", 1, "Parve", "צ'יפס", 0.14999999999999999, "Kilogram" },
                    { 2087, "Side", 1, "Parve", "קוגל בטטה", 1.0, "Portion" },
                    { 2088, "Dessert", 1, "Parve", "עוגת תפוחים", 1.0, "Portion" },
                    { 2089, "Dessert", 1, "Parve", "עוגת שיש", 1.0, "Portion" },
                    { 2090, "Dessert", 1, "Parve", "בראוניז", 1.0, "Unit" },
                    { 2091, "Dessert", 1, "Parve", "אבטיח", 0.29999999999999999, "Kilogram" },
                    { 2092, "Dessert", 1, "Parve", "פירות יבשים", 0.050000000000000003, "Kilogram" },
                    { 2093, "Dessert", 1, "Parve", "עוגת דבש", 1.0, "Portion" },
                    { 2094, "Drink", 1, "Parve", "לימונדה", 0.25, "Liter" },
                    { 2095, "Drink", 1, "Parve", "מיץ תפוחים", 0.25, "Liter" },
                    { 2096, "Drink", 1, "Parve", "תה קר", 0.25, "Liter" },
                    { 2097, "Drink", 1, "Parve", "סודה", 0.29999999999999999, "Liter" },
                    { 2098, "Drink", 1, "Parve", "משקה מוגז", 0.33000000000000002, "Liter" }
                });
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Dishes""', 'Id'), (SELECT MAX(""Id"") FROM ""Dishes""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2004);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2005);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2006);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2007);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2008);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2009);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2010);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2011);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2012);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2013);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2014);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2015);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2016);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2017);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2018);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2019);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2020);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2021);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2022);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2023);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2031);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2032);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2033);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2034);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2035);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2036);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2037);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2038);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2039);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2040);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2041);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2042);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2043);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2044);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2045);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2046);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2047);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2048);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2049);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2050);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2051);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2052);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2053);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2054);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2055);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2056);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2061);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2062);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2063);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2064);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2065);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2066);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2067);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2068);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2069);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2070);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2071);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2072);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2073);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2074);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2075);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2076);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2077);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2078);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2079);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2080);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2081);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2082);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2083);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2084);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2085);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2086);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2087);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2088);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2089);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2090);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2091);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2092);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2093);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2094);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2095);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2096);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2097);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2098);
        }
    }
}
