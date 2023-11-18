using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P03_FootballBetting.Data.Models;


namespace P03_FootballBetting.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Username)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(u => u.Password)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(u => u.Email)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(u => u.Name)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(u => u.Balance)
                .IsRequired(true);
        }
    }
}
