using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class SurgePricingRuleConfiguration : IEntityTypeConfiguration<SurgePricingRule>
{
    public void Configure(EntityTypeBuilder<SurgePricingRule> builder)
    {
        builder.Property(spr => spr.Name).HasMaxLength(100);
        builder.Property(spr => spr.TriggerThreshold).HasPrecision(18, 2);
        builder.Property(spr => spr.PriceMultiplier).HasPrecision(3, 2);
        builder.Property(spr => spr.MaxMultiplier).HasPrecision(3, 2).HasDefaultValue(3.0m);
    }
}
