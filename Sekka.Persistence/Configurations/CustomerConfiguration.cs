using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasIndex(c => new { c.DriverId, c.Phone }).IsUnique();
        builder.HasIndex(c => c.DriverId);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.AverageRating).HasPrecision(3, 2);
        builder.Property(c => c.BlockReason).HasMaxLength(200);
        builder.Property(c => c.Notes).HasMaxLength(1000);
        builder.HasOne(c => c.Driver).WithMany(d => d.Customers).HasForeignKey(c => c.DriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
