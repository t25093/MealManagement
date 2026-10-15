using MealManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasIndex(c => new { c.MealId, c.SentAt });

            builder.HasOne(c => c.Meal)
                .WithMany(m => m.Messages)
                .HasForeignKey(c => c.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}