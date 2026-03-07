using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class NotificationChannelPreferenceConfiguration : IEntityTypeConfiguration<Entities.NotificationChannelPreference>
{
    public void Configure(EntityTypeBuilder<Entities.NotificationChannelPreference> builder)
    {
        builder.HasIndex(n => new { n.DriverId, n.NotificationType }).IsUnique();
        builder.Property(n => n.SoundName).HasMaxLength(50).HasDefaultValue("default");
        builder.Property(n => n.VibrationPattern).HasMaxLength(50).HasDefaultValue("default");
        builder.Property(n => n.LedColor).HasMaxLength(10).HasDefaultValue("#FFFFFF");
    }
}
