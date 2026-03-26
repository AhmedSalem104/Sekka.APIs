using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class CancellationLogConfiguration : IEntityTypeConfiguration<CancellationLog>
{
    public void Configure(EntityTypeBuilder<CancellationLog> builder)
    {
        builder.HasIndex(c => c.OrderId).IsUnique();
        builder.Property(c => c.ReasonText).HasMaxLength(500);
        builder.Property(c => c.LossAmount).HasPrecision(18, 2);
        builder.Property(c => c.FuelCostLost).HasPrecision(18, 2);
        builder.Property(c => c.DocumentationUrl).HasMaxLength(500);

        builder.HasOne(c => c.Order)
            .WithOne(o => o.CancellationLog)
            .HasForeignKey<CancellationLog>(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.TransferredToDriver)
            .WithMany()
            .HasForeignKey(c => c.TransferredToDriverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
