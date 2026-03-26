using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class DeliveryTimeSlotConfiguration : IEntityTypeConfiguration<DeliveryTimeSlot>
{
    public void Configure(EntityTypeBuilder<DeliveryTimeSlot> builder)
    {
        builder.HasIndex(t => new { t.Date, t.StartTime, t.EndTime, t.RegionId });
        builder.Property(t => t.PriceMultiplier).HasPrecision(3, 2);
    }
}
