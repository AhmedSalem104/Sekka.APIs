using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    public void Configure(EntityTypeBuilder<PaymentRequest> builder)
    {
        builder.HasIndex(pr => pr.ReferenceCode).IsUnique();
        builder.HasIndex(pr => pr.DriverId);
        builder.HasIndex(pr => pr.Status);

        builder.Property(pr => pr.ReferenceCode).HasMaxLength(50);
        builder.Property(pr => pr.Amount).HasPrecision(18, 2);
        builder.Property(pr => pr.ProofImageUrl).HasMaxLength(500);
        builder.Property(pr => pr.SenderPhone).HasMaxLength(20);
        builder.Property(pr => pr.SenderName).HasMaxLength(100);
        builder.Property(pr => pr.Notes).HasMaxLength(500);
        builder.Property(pr => pr.AdminNotes).HasMaxLength(500);
        builder.Property(pr => pr.RelatedEntityType).HasMaxLength(50);

        builder.HasOne(pr => pr.Driver)
            .WithMany(d => d.PaymentRequests)
            .HasForeignKey(pr => pr.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.Admin)
            .WithMany()
            .HasForeignKey(pr => pr.AdminId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
