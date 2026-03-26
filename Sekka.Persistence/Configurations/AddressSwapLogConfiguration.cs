using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class AddressSwapLogConfiguration : IEntityTypeConfiguration<AddressSwapLog>
{
    public void Configure(EntityTypeBuilder<AddressSwapLog> builder)
    {
        builder.HasIndex(a => a.OrderId);
        builder.Property(a => a.OldAddress).HasMaxLength(500);
        builder.Property(a => a.NewAddress).HasMaxLength(500);
        builder.Property(a => a.Reason).HasMaxLength(200);
        builder.Property(a => a.CostDifference).HasPrecision(18, 2);

        builder.HasOne(a => a.Order)
            .WithMany(o => o.AddressSwapLogs)
            .HasForeignKey(a => a.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
