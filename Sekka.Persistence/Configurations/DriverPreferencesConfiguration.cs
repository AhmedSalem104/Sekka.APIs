using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class DriverPreferencesConfiguration : IEntityTypeConfiguration<Entities.DriverPreferences>
{
    public void Configure(EntityTypeBuilder<Entities.DriverPreferences> builder)
    {
        builder.HasIndex(p => p.DriverId).IsUnique();
        builder.Property(p => p.Language).HasMaxLength(5).HasDefaultValue("ar");
        builder.Property(p => p.TextToSpeechSpeed).HasPrecision(3, 2);
        builder.Property(p => p.HomeAddress).HasMaxLength(500);
        builder.Property(p => p.BackToBaseRadiusKm).HasPrecision(5, 2);
    }
}
