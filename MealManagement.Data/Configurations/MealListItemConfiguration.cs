using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class MealListItemConfiguration : IEntityTypeConfiguration<MealListItem>
    {
        public void Configure(EntityTypeBuilder<MealListItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Note)
                .HasMaxLength(200);

            builder.Property(i => i.ListType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(i => new { i.MealId, i.ListType });

            // מחיקת ארוחה מוחקת את הרשימות שלה
            builder.HasOne(i => i.Meal)
                .WithMany(m => m.ListItems)
                .HasForeignKey(i => i.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            // מחיקת משתמשת לא מוחקת פריטים: רק מנקה את השם שלה מהם
            builder.HasOne(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(i => i.DoneBy)
                .WithMany()
                .HasForeignKey(i => i.DoneByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
