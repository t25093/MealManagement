using MealManagement.Core.Entities;
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
        }
    }
}