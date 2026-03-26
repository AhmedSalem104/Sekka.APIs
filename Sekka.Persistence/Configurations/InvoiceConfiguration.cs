using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.DriverId);
        builder.HasIndex(i => i.PartnerId);
        builder.HasIndex(i => i.Status);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(30);
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2);
        builder.Property(i => i.TaxAmount).HasPrecision(18, 2);
        builder.Property(i => i.DiscountAmount).HasPrecision(18, 2);
        builder.Property(i => i.NetAmount).HasPrecision(18, 2);
        builder.Property(i => i.PdfUrl).HasMaxLength(500);
        builder.Property(i => i.Notes).HasMaxLength(500);

        builder.HasOne(i => i.Driver)
            .WithMany(d => d.Invoices)
            .HasForeignKey(i => i.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Partner)
            .WithMany()
            .HasForeignKey(i => i.PartnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
