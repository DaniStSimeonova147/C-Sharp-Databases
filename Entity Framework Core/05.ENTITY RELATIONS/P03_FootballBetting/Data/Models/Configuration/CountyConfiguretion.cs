using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data.Configurations
{
  public  class CountyConfiguretion : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(c => c.CountryId);

            builder.Property(c => c.Name)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsUnicode(true);
        }
    }
}
