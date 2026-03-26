using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasIndex(v => v.DriverId);

        builder.Property(v => v.PlateNumber).HasMaxLength(20);
        builder.Property(v => v.MakeModel).HasMaxLength(100);
        builder.Property(v => v.FuelConsumptionPer100Km).HasPrecision(5, 2);
        builder.Property(v => v.FuelPricePerLiter).HasPrecision(18, 2);
        builder.Property(v => v.RejectionReason).HasMaxLength(500);
        builder.Property(v => v.ApprovedBy).HasMaxLength(100);

        builder.HasOne(v => v.Driver)
            .WithMany(d => d.Vehicles)
            .HasForeignKey(v => v.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
