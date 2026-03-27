using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class SavingsCirclePaymentConfiguration : IEntityTypeConfiguration<SavingsCirclePayment>
{
    public void Configure(EntityTypeBuilder<SavingsCirclePayment> builder)
    {
        builder.HasIndex(p => new { p.CircleId, p.MemberId, p.RoundNumber }).IsUnique();
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.HasOne(p => p.Circle).WithMany(c => c.Payments).HasForeignKey(p => p.CircleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.Member).WithMany().HasForeignKey(p => p.MemberId).OnDelete(DeleteBehavior.Restrict);
    }
}
