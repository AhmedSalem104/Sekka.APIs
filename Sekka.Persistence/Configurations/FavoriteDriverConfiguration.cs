using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class FavoriteDriverConfiguration : IEntityTypeConfiguration<FavoriteDriver>
{
    public void Configure(EntityTypeBuilder<FavoriteDriver> builder)
    {
        builder.Property(f => f.Name).IsRequired().HasMaxLength(50);
        builder.Property(f => f.Phone).IsRequired().HasMaxLength(15);

        builder.HasIndex(f => new { f.DriverId, f.Phone }).IsUnique();

        builder.HasOne(f => f.Driver)
            .WithMany(d => d.FavoriteDrivers)
            .HasForeignKey(f => f.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.LinkedDriver)
            .WithMany()
            .HasForeignKey(f => f.LinkedDriverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
