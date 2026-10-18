using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class DishConfiguration : IEntityTypeConfiguration<Dish>
    {
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Category)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(d => d.KosherType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(d => d.Unit)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(d => d.QuantityPerPerson)
                .HasPrecision(10, 2);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            const int adminId = 1;

            builder.HasData(
                new Dish { Id = 1, Name = "חומוס", Category = DishCategory.Starter,  KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2, Name = "טחינה", Category = DishCategory.Starter,  KosherType = MealType.Parve, QuantityPerPerson = 0.05, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 3, Name = "גפילטע פיש", Category = DishCategory.Starter,  KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 4, Name = "סלט ירקות", Category = DishCategory.Salad,  KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 5, Name = "סלט כרוב", Category = DishCategory.Salad,  KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 6, Name = "סלט חצילים", Category = DishCategory.Salad,  KosherType = MealType.Parve, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 7, Name = "עוף צלוי", Category = DishCategory.Main,  KosherType = MealType.Meat, QuantityPerPerson = 0.30, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 8, Name = "צלי בקר", Category = DishCategory.Main,  KosherType = MealType.Meat, QuantityPerPerson = 0.20, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 9, Name = "קציצות בשר", Category = DishCategory.Main,  KosherType = MealType.Meat, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 10, Name = "שניצל", Category = DishCategory.Main,  KosherType = MealType.Meat, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 11, Name = "לזניה", Category = DishCategory.Main,  KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 12, Name = "קיש בצל", Category = DishCategory.Main,  KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 13, Name = "אורז", Category = DishCategory.Side,  KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 14, Name = "קוגל תפוחי אדמה", Category = DishCategory.Side,  KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 15, Name = "קוגל ירושלמי", Category = DishCategory.Side,  KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 16, Name = "תפוחי אדמה אפויים", Category = DishCategory.Side,  KosherType = MealType.Parve, QuantityPerPerson = 0.20, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 17, Name = "עוגת שוקולד", Category = DishCategory.Dessert,  KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 18, Name = "עוגת גבינה", Category = DishCategory.Dessert,  KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 19, Name = "סלט פירות", Category = DishCategory.Dessert,  KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 20, Name = "מיץ ענבים", Category = DishCategory.Drink,  KosherType = MealType.Parve, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId });

            // מאגר מורחב לכל סוג ארוחה (שינוי לאחר הסיום).
            // מזהים מ-1001 כדי לא להתנגש במאכלים שמשתמשות כבר הוסיפו (שקיבלו 21 ומעלה).
            builder.HasData(
                // ===== מאגר נוסף: בשרי =====
                new Dish { Id = 1001, Name = "כבד קצוץ", Category = DishCategory.Starter, KosherType = MealType.Meat, QuantityPerPerson = 0.05, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1002, Name = "מרק עוף", Category = DishCategory.Starter, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1003, Name = "סלט עוף", Category = DishCategory.Salad, KosherType = MealType.Meat, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1004, Name = "שוקיים בתנור", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 1005, Name = "גולאש", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.25, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1006, Name = "חמין", Category = DishCategory.Side, KosherType = MealType.Meat, QuantityPerPerson = 0.25, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1007, Name = "אורז עם בשר טחון", Category = DishCategory.Side, KosherType = MealType.Meat, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                // ===== מאגר נוסף: חלבי =====
                new Dish { Id = 1011, Name = "בורקס גבינה", Category = DishCategory.Starter, KosherType = MealType.Dairy, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 1012, Name = "סלט יווני", Category = DishCategory.Salad, KosherType = MealType.Dairy, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1013, Name = "פשטידת ירקות", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1014, Name = "פסטה ברוטב שמנת", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1015, Name = "בלינצ'ס גבינה", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 1016, Name = "תפוחי אדמה מוקרמים", Category = DishCategory.Side, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1017, Name = "מוס שוקולד", Category = DishCategory.Dessert, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1018, Name = "קפה קר", Category = DishCategory.Drink, KosherType = MealType.Dairy, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                // ===== מאגר נוסף: פרווה =====
                new Dish { Id = 1021, Name = "מטבוחה", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 0.05, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1022, Name = "סלט חסה", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1023, Name = "סלט תפוחי אדמה", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1024, Name = "ירקות מוקפצים", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1025, Name = "קוגל אטריות", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1026, Name = "שעועית ירוקה", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 1027, Name = "עוגיות", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 1028, Name = "סורבה", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 1029, Name = "מים מינרליים", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.50, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 1030, Name = "מיץ תפוזים", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId });

            // מאגר מורחב 2 (מזהים 2001 עד 2098).
            // לא 1031: אחרי מאגר 1 המונה עלה ל-1030, ומאכל שנוסף מהאתר קיבל 1031.
            builder.HasData(
                // ===== מאגר מורחב 2: בשרי =====
                new Dish { Id = 2001, Name = "סיגרים בבשר", Category = DishCategory.Starter, KosherType = MealType.Meat, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2002, Name = "קובה", Category = DishCategory.Starter, KosherType = MealType.Meat, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2003, Name = "כנפיים ברוטב", Category = DishCategory.Starter, KosherType = MealType.Meat, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2004, Name = "מרק בקר וירקות", Category = DishCategory.Starter, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2005, Name = "סלט פסטרמה", Category = DishCategory.Salad, KosherType = MealType.Meat, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2006, Name = "סלט עוף אסייתי", Category = DishCategory.Salad, KosherType = MealType.Meat, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2007, Name = "פשטידת בשר", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2008, Name = "שניצל עוף אפוי", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2009, Name = "חזה עוף בגריל", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2010, Name = "פרגיות על האש", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.25, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2011, Name = "צלי הודו", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.20, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2012, Name = "בשר בקר בתנור", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.20, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2013, Name = "קציצות עוף ברוטב", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2014, Name = "עוף בתפוזים", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.30, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2015, Name = "ממולאים בבשר", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2016, Name = "לשון ברוטב", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2017, Name = "נקניקיות", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2018, Name = "פסטה בולונז", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2019, Name = "מוסקה בשרית", Category = DishCategory.Main, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2020, Name = "פתיתים עם עוף", Category = DishCategory.Side, KosherType = MealType.Meat, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2021, Name = "קישקע", Category = DishCategory.Side, KosherType = MealType.Meat, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2022, Name = "שעועית עם בשר", Category = DishCategory.Side, KosherType = MealType.Meat, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2023, Name = "קוסקוס עם מרק עוף", Category = DishCategory.Side, KosherType = MealType.Meat, QuantityPerPerson = 0.20, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                // ===== מאגר מורחב 2: חלבי =====
                new Dish { Id = 2031, Name = "מרק שמנת פטריות", Category = DishCategory.Starter, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2032, Name = "מגש גבינות", Category = DishCategory.Starter, KosherType = MealType.Dairy, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2033, Name = "טוסטים קטנים", Category = DishCategory.Starter, KosherType = MealType.Dairy, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2034, Name = "מרק בטטה בשמנת", Category = DishCategory.Starter, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2035, Name = "סלט קפרזה", Category = DishCategory.Salad, KosherType = MealType.Dairy, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2036, Name = "סלט קיסר", Category = DishCategory.Salad, KosherType = MealType.Dairy, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2037, Name = "סלט פסטה עם גבינה", Category = DishCategory.Salad, KosherType = MealType.Dairy, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2038, Name = "פיצה", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2039, Name = "רביולי גבינה", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2040, Name = "פסטה ברוטב רוזה", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2041, Name = "קיש פטריות", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2042, Name = "פשטידת ברוקולי", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2043, Name = "שקשוקה עם גבינה", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2044, Name = "ניוקי ברוטב שמנת", Category = DishCategory.Main, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2045, Name = "לחמניות גבינה", Category = DishCategory.Side, KosherType = MealType.Dairy, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2046, Name = "פירה בחמאה", Category = DishCategory.Side, KosherType = MealType.Dairy, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2047, Name = "אורז מוקרם", Category = DishCategory.Side, KosherType = MealType.Dairy, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2048, Name = "ירקות מוקרמים", Category = DishCategory.Side, KosherType = MealType.Dairy, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2049, Name = "עוגת גבינה פירורים", Category = DishCategory.Dessert, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2050, Name = "גלידה", Category = DishCategory.Dessert, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2051, Name = "מלבי", Category = DishCategory.Dessert, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2052, Name = "עוגת שמרים", Category = DishCategory.Dessert, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2053, Name = "פנקייקים", Category = DishCategory.Dessert, KosherType = MealType.Dairy, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2054, Name = "שוקו", Category = DishCategory.Drink, KosherType = MealType.Dairy, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 2055, Name = "מילקשייק", Category = DishCategory.Drink, KosherType = MealType.Dairy, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 2056, Name = "קפה הפוך", Category = DishCategory.Drink, KosherType = MealType.Dairy, QuantityPerPerson = 0.20, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                // ===== מאגר מורחב 2: פרווה =====
                new Dish { Id = 2061, Name = "פלאפל", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2062, Name = "חציל בטחינה", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2063, Name = "ממרח פלפלים קלויים", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 0.05, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2064, Name = "זיתים", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 0.04, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2065, Name = "מרק ירקות", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2066, Name = "מרק עדשים", Category = DishCategory.Starter, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2067, Name = "סלט טבולה", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2068, Name = "סלט גזר", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2069, Name = "סלט סלק", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2070, Name = "סלט קינואה", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2071, Name = "סלט פסטה", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2072, Name = "סלט כרוב סגול", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2073, Name = "סלט מלפפונים", Category = DishCategory.Salad, KosherType = MealType.Parve, QuantityPerPerson = 0.08, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2074, Name = "שקשוקה", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2075, Name = "פשטידת תרד", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2076, Name = "מג'דרה", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 0.20, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2077, Name = "ממולאים באורז", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2078, Name = "קציצות ירק", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2079, Name = "פסטה ברוטב עגבניות", Category = DishCategory.Main, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2080, Name = "חלה", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2081, Name = "פתיתים", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.12, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2082, Name = "קוסקוס", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2083, Name = "בטטה בתנור", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2084, Name = "תירס", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2085, Name = "פירה", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2086, Name = "צ'יפס", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 0.15, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2087, Name = "קוגל בטטה", Category = DishCategory.Side, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2088, Name = "עוגת תפוחים", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2089, Name = "עוגת שיש", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2090, Name = "בראוניז", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 2091, Name = "אבטיח", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 0.30, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2092, Name = "פירות יבשים", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 0.05, Unit = MeasureUnit.Kilogram, CreatedByUserId = adminId },
                new Dish { Id = 2093, Name = "עוגת דבש", Category = DishCategory.Dessert, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 2094, Name = "לימונדה", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 2095, Name = "מיץ תפוחים", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 2096, Name = "תה קר", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.25, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 2097, Name = "סודה", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.30, Unit = MeasureUnit.Liter, CreatedByUserId = adminId },
                new Dish { Id = 2098, Name = "משקה מוגז", Category = DishCategory.Drink, KosherType = MealType.Parve, QuantityPerPerson = 0.33, Unit = MeasureUnit.Liter, CreatedByUserId = adminId });

            // קטגוריות חדשות: בר עוגות וחד פעמי (מזהים 3001 ומעלה)
            builder.HasData(
                // ===== בר עוגות =====
                new Dish { Id = 3001, Name = "מגש עוגיות", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3002, Name = "מאפינס", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3003, Name = "רולדה", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 3004, Name = "עוגיות שוקולד צ'יפס", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3005, Name = "כדורי שוקולד", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3006, Name = "עוגת גזר", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 3007, Name = "טארט פירות", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 3008, Name = "פחזניות", Category = DishCategory.CakeBar, KosherType = MealType.Parve, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3009, Name = "עוגת גבינה אפויה", Category = DishCategory.CakeBar, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                new Dish { Id = 3010, Name = "טירמיסו", Category = DishCategory.CakeBar, KosherType = MealType.Dairy, QuantityPerPerson = 1, Unit = MeasureUnit.Portion, CreatedByUserId = adminId },
                // ===== חד פעמי =====
                new Dish { Id = 3021, Name = "צלחות חד פעמיות גדולות", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3022, Name = "צלחות חד פעמיות קטנות", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 2, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3023, Name = "כוסות חד פעמיות", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3024, Name = "סט סכין, מזלג וכף", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3025, Name = "מפיות", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 3, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3026, Name = "קערות חד פעמיות", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 1, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3027, Name = "מפות שולחן חד פעמיות", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Unit, CreatedByUserId = adminId },
                new Dish { Id = 3028, Name = "מגשי הגשה", Category = DishCategory.Disposables, KosherType = MealType.Parve, QuantityPerPerson = 0.10, Unit = MeasureUnit.Unit, CreatedByUserId = adminId });
        }
    }
}