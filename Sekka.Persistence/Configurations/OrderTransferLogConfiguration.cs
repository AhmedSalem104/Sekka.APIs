using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class OrderTransferLogConfiguration : IEntityTypeConfiguration<OrderTransferLog>
{
    public void Configure(EntityTypeBuilder<OrderTransferLog> builder)
    {
        builder.HasIndex(t => t.OrderId);
        builder.HasIndex(t => t.DeepLinkToken).IsUnique().HasFilter("[DeepLinkToken] IS NOT NULL");
        builder.Property(t => t.TransferReason).HasMaxLength(200);
        builder.Property(t => t.DeepLinkToken).HasMaxLength(100);

        builder.HasOne(t => t.Order)
            .WithMany(o => o.TransferLogs)
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.FromDriver)
            .WithMany()
            .HasForeignKey(t => t.FromDriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ToDriver)
            .WithMany()
            .HasForeignKey(t => t.ToDriverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
