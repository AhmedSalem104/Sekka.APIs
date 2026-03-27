using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasIndex(s => s.DriverId);
        builder.HasIndex(s => s.PlanId);
        builder.HasIndex(s => new { s.DriverId, s.Status });
        builder.Property(s => s.AutoRenew).HasDefaultValue(true);
        builder.HasOne(s => s.Driver).WithMany(d => d.Subscriptions).HasForeignKey(s => s.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Plan).WithMany(p => p.Subscriptions).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.PaymentRequest).WithMany().HasForeignKey(s => s.PaymentRequestId).OnDelete(DeleteBehavior.SetNull);
    }
}
