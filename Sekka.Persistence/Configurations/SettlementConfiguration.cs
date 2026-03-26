using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.HasIndex(s => s.DriverId);
        builder.HasIndex(s => s.PartnerId);

        builder.Property(s => s.Amount).HasPrecision(18, 2);
        builder.Property(s => s.Notes).HasMaxLength(500);
        builder.Property(s => s.ReceiptImageUrl).HasMaxLength(500);

        builder.HasOne(s => s.Driver)
            .WithMany(d => d.Settlements)
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Partner)
            .WithMany()
            .HasForeignKey(s => s.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
