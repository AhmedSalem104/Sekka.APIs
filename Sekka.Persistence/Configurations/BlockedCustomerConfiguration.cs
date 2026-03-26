using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class BlockedCustomerConfiguration : IEntityTypeConfiguration<BlockedCustomer>
{
    public void Configure(EntityTypeBuilder<BlockedCustomer> builder)
    {
        builder.HasIndex(b => new { b.DriverId, b.CustomerPhone }).IsUnique();
        builder.Property(b => b.CustomerPhone).HasMaxLength(20);
        builder.Property(b => b.Reason).HasMaxLength(200);
        builder.HasOne(b => b.Driver).WithMany(d => d.BlockedCustomers).HasForeignKey(b => b.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Customer).WithMany(c => c.BlockedEntries).HasForeignKey(b => b.CustomerId).OnDelete(DeleteBehavior.SetNull);
    }
}
