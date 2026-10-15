using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
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
            .UsingEntity(j =>
            {
                j.ToTable("MealParticipants");
                j.HasData(new { ParticipantsId = 2, ParticipatingMealsId = 1 });
            });

            builder.HasData(new Meal
            {
                Id = 1,
                Name = "ארוחת ערב משפחתית",
                MealType = MealType.Meat,
                EventDate = new DateTime(2026, 12, 1, 17, 0, 0, DateTimeKind.Utc),
                GuestsCount = 30,
                OwnerId = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}