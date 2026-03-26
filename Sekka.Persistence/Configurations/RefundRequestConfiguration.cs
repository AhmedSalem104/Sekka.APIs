using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class RefundRequestConfiguration : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(EntityTypeBuilder<RefundRequest> builder)
    {
        builder.HasIndex(rr => rr.OrderId);
        builder.HasIndex(rr => rr.DriverId);
        builder.HasIndex(rr => rr.Status);

        builder.Property(rr => rr.Amount).HasPrecision(18, 2);
        builder.Property(rr => rr.Description).HasMaxLength(500);
        builder.Property(rr => rr.AdminNotes).HasMaxLength(500);
        builder.Property(rr => rr.ProcessedBy).HasMaxLength(100);

        builder.HasOne(rr => rr.Order)
            .WithMany()
            .HasForeignKey(rr => rr.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rr => rr.Driver)
            .WithMany(d => d.RefundRequests)
            .HasForeignKey(rr => rr.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
