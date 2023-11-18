using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(t => t.TeamId);

            builder.Property(t => t.Name)
                .HasMaxLength(80)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(t => t.Initials)
                .HasMaxLength(3)
                .IsRequired(true)
                .IsUnicode(true);

            builder.Property(t => t.Budget)
                .IsRequired(true);

            builder.Property(t => t.PrimaryKitColorId)
                .IsRequired(true);

            builder
                .HasOne(t => t.PrimaryKitColor)
                .WithMany(pk => pk.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.SecondaryKitColorId)
                .IsRequired(true);

            builder
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(sk => sk.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.TownId)
                .IsRequired(true);

            builder
                .HasOne(t => t.Town)
                .WithMany(tw => tw.Teams)
                .HasForeignKey(t => t.TownId);
        }
    }
}
