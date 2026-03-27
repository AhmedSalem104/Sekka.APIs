using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.Property(f => f.FeatureKey).IsRequired().HasMaxLength(100);
        builder.Property(f => f.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(f => f.DisplayNameEn).HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(500);
        builder.Property(f => f.MinAppVersion).HasMaxLength(20);
        builder.Property(f => f.Category).HasMaxLength(50);
        builder.Property(f => f.CreatedBy).HasMaxLength(100);
        builder.Property(f => f.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(f => f.FeatureKey).IsUnique();
        builder.HasIndex(f => f.IsEnabled);
        builder.HasIndex(f => f.Category);
    }
}
