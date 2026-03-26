using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasIndex(a => a.DriverId);
        builder.HasIndex(a => a.CustomerId);
        builder.Property(a => a.AddressText).HasMaxLength(500);
        builder.Property(a => a.Landmarks).HasMaxLength(500);
        builder.Property(a => a.DeliveryNotes).HasMaxLength(500);
        builder.HasOne(a => a.Driver).WithMany(d => d.Addresses).HasForeignKey(a => a.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Customer).WithMany(c => c.Addresses).HasForeignKey(a => a.CustomerId).OnDelete(DeleteBehavior.SetNull);
    }
}
