using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.MealType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(m => m.EventDate)
                .HasColumnType("timestamp with time zone");

            builder.HasOne(m => m.Owner)
                .WithMany(u => u.OwnedMeals)
                .HasForeignKey(m => m.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Participants)
                .WithMany(u => u.ParticipatingMeals)
                .UsingEntity(j => j.ToTable("MealParticipants"));
        }
    }
}