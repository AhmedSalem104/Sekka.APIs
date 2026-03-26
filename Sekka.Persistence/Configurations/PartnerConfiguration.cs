using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.HasIndex(p => p.DriverId);
        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.Property(p => p.Address).HasMaxLength(500);
        builder.Property(p => p.CommissionValue).HasPrecision(18, 2);
        builder.Property(p => p.Color).HasMaxLength(10).HasDefaultValue("#3B82F6");
        builder.Property(p => p.LogoUrl).HasMaxLength(500);
        builder.Property(p => p.ReceiptHeader).HasMaxLength(200);
        builder.Property(p => p.VerificationDocumentUrl).HasMaxLength(500);
        builder.Property(p => p.VerificationNote).HasMaxLength(500);
        builder.HasOne(p => p.Driver).WithMany(d => d.Partners).HasForeignKey(p => p.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.VerifiedByAdmin).WithMany().HasForeignKey(p => p.VerifiedByAdminId).OnDelete(DeleteBehavior.SetNull);
    }
}
