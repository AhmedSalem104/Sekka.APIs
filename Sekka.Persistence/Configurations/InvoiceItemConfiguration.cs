using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasIndex(ii => ii.InvoiceId);

        builder.Property(ii => ii.Description).HasMaxLength(200);
        builder.Property(ii => ii.UnitPrice).HasPrecision(18, 2);
        builder.Property(ii => ii.TotalPrice).HasPrecision(18, 2);

        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ii => ii.Order)
            .WithMany()
            .HasForeignKey(ii => ii.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
