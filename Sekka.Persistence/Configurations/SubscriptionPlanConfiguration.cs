using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(50);
        builder.Property(p => p.NameEn).HasMaxLength(50);
        builder.Property(p => p.PriceMonthly).HasPrecision(18, 2);
        builder.Property(p => p.PriceAnnual).HasPrecision(18, 2);
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.SortOrder).HasDefaultValue(0);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.SortOrder);
    }
}
