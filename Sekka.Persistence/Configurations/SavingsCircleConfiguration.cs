using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class SavingsCircleConfiguration : IEntityTypeConfiguration<SavingsCircle>
{
    public void Configure(EntityTypeBuilder<SavingsCircle> builder)
    {
        builder.HasIndex(c => c.CreatorDriverId);
        builder.HasIndex(c => c.Status);
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.MonthlyAmount).HasPrecision(18, 2);
        builder.Property(c => c.CurrentRound).HasDefaultValue(0);
        builder.Property(c => c.MinHealthScore).HasDefaultValue(80);
        builder.HasOne(c => c.Creator).WithMany(d => d.SavingsCirclesCreated).HasForeignKey(c => c.CreatorDriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
