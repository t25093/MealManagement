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
        }
    }
}