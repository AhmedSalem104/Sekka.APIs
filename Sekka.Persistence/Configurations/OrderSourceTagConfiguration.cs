using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class OrderSourceTagConfiguration : IEntityTypeConfiguration<OrderSourceTag>
{
    public void Configure(EntityTypeBuilder<OrderSourceTag> builder)
    {
        builder.HasIndex(s => s.OrderId).IsUnique();
        builder.Property(s => s.SourceName).HasMaxLength(100);
        builder.Property(s => s.SourceReference).HasMaxLength(200);

        builder.HasOne(s => s.Order)
            .WithOne(o => o.SourceTag)
            .HasForeignKey<OrderSourceTag>(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
