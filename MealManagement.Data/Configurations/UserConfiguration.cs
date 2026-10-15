using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealManagement.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasData(
                    new User
                    {
                        Id = 1,
                        FullName = "מנהלת המערכת",
                        Email = "admin@meals.local",
                        // הסיסמה: Admin123!
                        PasswordHash = "$2b$11$XuY3CT.E4JB3LVUquipoXeaoDF8voHOA1hx6nBShMB6tZQfD.etxm",
                        Role = UserRole.Admin,
                        CreateAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                     new User
                     {
                         Id = 2,
                         FullName = "משתמשת לדוגמה",
                         Email = "user@meals.local",
                         // הסיסמה: User123!
                         PasswordHash = "$2b$11$bQ5cSLxZkd1tjqGEsgqKl.r88vIa6nBg2kTWoEbP.gLo8.v3UTCpa",
                         Role = UserRole.User,
                         CreateAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                     });
        }
    }
}