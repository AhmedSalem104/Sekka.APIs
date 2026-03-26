using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class ParkingSpotConfiguration : IEntityTypeConfiguration<ParkingSpot>
{
    public void Configure(EntityTypeBuilder<ParkingSpot> builder)
    {
        builder.HasIndex(p => p.DriverId);

        builder.Property(p => p.Address).HasMaxLength(500);
        builder.Property(p => p.Notes).HasMaxLength(200);
        builder.Property(p => p.PaidAmount).HasPrecision(18, 2);

        builder.HasOne(p => p.Driver)
            .WithMany(d => d.ParkingSpots)
            .HasForeignKey(p => p.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
