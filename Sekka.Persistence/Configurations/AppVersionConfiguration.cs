using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class AppVersionConfiguration : IEntityTypeConfiguration<AppVersion>
{
    public void Configure(EntityTypeBuilder<AppVersion> builder)
    {
        builder.Property(v => v.VersionCode).IsRequired().HasMaxLength(20);
        builder.Property(v => v.MinRequiredVersion).IsRequired().HasMaxLength(20);
        builder.Property(v => v.StoreUrl).IsRequired().HasMaxLength(500);
        builder.Property(v => v.ReleaseNotes).HasMaxLength(2000);
        builder.Property(v => v.ReleaseNotesEn).HasMaxLength(2000);
        builder.Property(v => v.ReleasedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(v => v.Platform);
        builder.HasIndex(v => v.IsActive);
        builder.HasIndex(v => new { v.Platform, v.VersionCode }).IsUnique();
    }
}
