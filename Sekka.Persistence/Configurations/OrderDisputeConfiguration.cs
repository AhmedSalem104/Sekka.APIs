using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class OrderDisputeConfiguration : IEntityTypeConfiguration<OrderDispute>
{
    public void Configure(EntityTypeBuilder<OrderDispute> builder)
    {
        builder.HasIndex(od => od.OrderId);
        builder.HasIndex(od => od.DriverId);
        builder.HasIndex(od => od.Status);

        builder.Property(od => od.Description).HasMaxLength(1000);
        builder.Property(od => od.AdminNotes).HasMaxLength(1000);
        builder.Property(od => od.Resolution).HasMaxLength(1000);
        builder.Property(od => od.ResolvedBy).HasMaxLength(100);

        builder.HasOne(od => od.Order)
            .WithMany()
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(od => od.Driver)
            .WithMany(d => d.OrderDisputes)
            .HasForeignKey(od => od.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
