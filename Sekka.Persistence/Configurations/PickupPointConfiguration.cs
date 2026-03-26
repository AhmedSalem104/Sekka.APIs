using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class PickupPointConfiguration : IEntityTypeConfiguration<PickupPoint>
{
    public void Configure(EntityTypeBuilder<PickupPoint> builder)
    {
        builder.HasIndex(p => p.PartnerId);
        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(p => p.Address).HasMaxLength(500);
        builder.Property(p => p.DriverRating).HasPrecision(3, 2);
        builder.Property(p => p.Notes).HasMaxLength(500);
        builder.HasOne(p => p.Partner).WithMany(pa => pa.PickupPoints).HasForeignKey(p => p.PartnerId).OnDelete(DeleteBehavior.Cascade);
    }
}
