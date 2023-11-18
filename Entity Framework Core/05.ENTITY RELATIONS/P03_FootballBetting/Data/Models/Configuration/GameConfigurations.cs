using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P03_FootballBetting.Data.Models;


namespace P03_FootballBetting.Data.Configurations
{
    public class GameConfigurations : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(g => g.GameId);

            builder.Property(g => g.HomeTeamGoals)
                .IsRequired(true);

            builder.Property(g => g.AwayTeamGoals)
                .IsRequired(true);

            builder.Property(g => g.DateTime)
                .IsRequired(true);

            builder.Property(g => g.HomeTeamBetRate)
                .IsRequired(true);

            builder.Property(g => g.AwayTeamBetRate)
                .IsRequired(true);

            builder.Property(g => g.DrawBetRate)
                .IsRequired(true);

            builder.Property(g => g.Result)
                .IsRequired(true);

            builder.Property(g => g.HomeTeamId)
                .IsRequired(true);

            builder
                .HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(g => g.AwayTeamId)
                .IsRequired(true);

            builder
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
