using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class DeliveryAttemptConfiguration : IEntityTypeConfiguration<DeliveryAttempt>
{
    public void Configure(EntityTypeBuilder<DeliveryAttempt> builder)
    {
        builder.HasIndex(d => new { d.OrderId, d.AttemptNumber });
        builder.Property(d => d.Reason).HasMaxLength(500);
        builder.Property(d => d.Notes).HasMaxLength(500);

        builder.HasOne(d => d.Order)
            .WithMany(o => o.DeliveryAttempts)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
