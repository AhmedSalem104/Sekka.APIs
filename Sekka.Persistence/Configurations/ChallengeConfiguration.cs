using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class ChallengeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.TargetMetric).HasMaxLength(50);
        builder.Property(c => c.TargetValue).HasPrecision(18, 2);
        builder.Property(c => c.BadgeName).HasMaxLength(50);
        builder.Property(c => c.BadgeIconUrl).HasMaxLength(500);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.ChallengeType);
    }
}
