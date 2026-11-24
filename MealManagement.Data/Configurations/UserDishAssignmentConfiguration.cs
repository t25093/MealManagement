using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class UserDishAssignmentConfiguration : IEntityTypeConfiguration<UserDishAssignment>
    {
        public void Configure(EntityTypeBuilder<UserDishAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Quantity)
                .HasPrecision(10, 2);

            builder.HasIndex(a => new { a.MealDishId, a.UserId })
                .IsUnique();

            builder.Property(a => a.PendingEmail)
                .HasMaxLength(256);

            builder.HasIndex(a => a.PendingEmail);

            builder.HasOne(a => a.MealDish)
                .WithMany(md => md.Assignments)
                .HasForeignKey(a => a.MealDishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}