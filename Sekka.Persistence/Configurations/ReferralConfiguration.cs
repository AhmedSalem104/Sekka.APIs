using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.HasIndex(r => r.ReferralCode).IsUnique();
        builder.HasIndex(r => r.ReferrerDriverId);
        builder.HasIndex(r => r.ReferredDriverId);
        builder.Property(r => r.ReferralCode).HasMaxLength(20);
        builder.Property(r => r.ReferredPhone).HasMaxLength(20);
        builder.Property(r => r.RewardGiven).HasDefaultValue(false);
        builder.Property(r => r.ReferrerRewardGiven).HasDefaultValue(false);
        builder.HasOne(r => r.ReferrerDriver).WithMany(d => d.ReferralsSent).HasForeignKey(r => r.ReferrerDriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.ReferredDriver).WithMany(d => d.ReferralsReceived).HasForeignKey(r => r.ReferredDriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
