using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.IdempotencyKey).IsUnique().HasFilter("[IdempotencyKey] IS NOT NULL");
        builder.HasIndex(o => o.DriverId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
        builder.HasIndex(o => new { o.DriverId, o.Status });
        builder.HasIndex(o => new { o.DriverId, o.CreatedAt });

        builder.Property(o => o.OrderNumber).HasMaxLength(20);
        builder.Property(o => o.Description).HasMaxLength(500);
        builder.Property(o => o.Amount).HasPrecision(18, 2);
        builder.Property(o => o.CommissionAmount).HasPrecision(18, 2);
        builder.Property(o => o.PickupAddress).HasMaxLength(500);
        builder.Property(o => o.DeliveryAddress).HasMaxLength(500);
        builder.Property(o => o.Notes).HasMaxLength(1000);
        builder.Property(o => o.RecurrencePattern).HasMaxLength(50);
        builder.Property(o => o.ExpectedChangeAmount).HasPrecision(18, 2);
        builder.Property(o => o.ActualCollectedAmount).HasPrecision(18, 2);
        builder.Property(o => o.ReturnReason).HasMaxLength(200);
        builder.Property(o => o.PartialDeliveryNote).HasMaxLength(500);
        builder.Property(o => o.IdempotencyKey).HasMaxLength(64);
        builder.Property(o => o.WorthScore).HasPrecision(5, 2);

        builder.HasOne(o => o.Driver)
            .WithMany(d => d.Orders)
            .HasForeignKey(o => o.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
