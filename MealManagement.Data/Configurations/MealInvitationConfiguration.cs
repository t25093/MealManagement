using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class MealInvitationConfiguration : IEntityTypeConfiguration<MealInvitation>
    {
        public void Configure(EntityTypeBuilder<MealInvitation> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(200);

            // אותו אימייל לא מוזמן פעמיים לאותה ארוחה
            builder.HasIndex(i => new { i.MealId, i.Email })
                .IsUnique();

            // לחיפוש מהיר בזמן הרשמה
            builder.HasIndex(i => i.Email);

            builder.HasOne(i => i.Meal)
                .WithMany(m => m.Invitations)
                .HasForeignKey(i => i.MealId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
