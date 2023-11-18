using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data.Configurations
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(p => p.PlayerId);

            builder.Property(p => p.Name)
                .HasMaxLength(80)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(p => p.SquadNumber)
                .IsRequired(true);

            builder.Property(p => p.IsInjured)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder.Property(p => p.TeamId)
                .IsRequired(true);

            builder
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            builder.Property(p => p.PositionId)
                .IsRequired(true);

            builder
                .HasOne(p => p.Position)
                .WithMany(ps => ps.Players)
                .HasForeignKey(p => p.PositionId);
        }
    }
}
