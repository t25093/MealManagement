using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Content)
                .IsRequired()
                .HasMaxLength(5000);

            builder.HasOne(r => r.Meal)
                .WithMany(m => m.Recipes)
                .HasForeignKey(r => r.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Dish)
                .WithMany()
                .HasForeignKey(r => r.DishId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}