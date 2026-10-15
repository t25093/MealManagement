using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class MealDishConfiguration : IEntityTypeConfiguration<MealDish>
    {
        public void Configure(EntityTypeBuilder<MealDish> builder)
        {
            builder.HasKey(md => md.Id);

            builder.Property(md => md.Category)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(md => md.RequiredQuantity)
                .HasPrecision(10, 2);

            builder.Property(md => md.AssignedQuantity)
                .HasPrecision(10, 2);

            builder.HasOne(md => md.Meal)
                .WithMany(m => m.MealDishes)
                .HasForeignKey(md => md.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(md => md.Dish)
                .WithMany(d => d.MealDishes)
                .HasForeignKey(md => md.DishId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // concurrency token: ממופה לעמודת המערכת xmin של PostgreSQL
            builder.Property(md => md.Version)
                .IsRowVersion();
        }
    }
}