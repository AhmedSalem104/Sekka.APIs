using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class DailyStatsConfiguration : IEntityTypeConfiguration<DailyStats>
{
    public void Configure(EntityTypeBuilder<DailyStats> builder)
    {
        builder.HasIndex(ds => new { ds.DriverId, ds.Date }).IsUnique();

        builder.Property(ds => ds.TotalEarnings).HasPrecision(18, 2);
        builder.Property(ds => ds.TotalCommissions).HasPrecision(18, 2);
        builder.Property(ds => ds.TotalExpenses).HasPrecision(18, 2);
        builder.Property(ds => ds.NetProfit).HasPrecision(18, 2);
        builder.Property(ds => ds.CashCollected).HasPrecision(18, 2);
        builder.Property(ds => ds.BestRegion).HasMaxLength(100);
        builder.Property(ds => ds.BestTimeSlot).HasMaxLength(50);

        builder.HasOne(ds => ds.Driver)
            .WithMany(d => d.DailyStats)
            .HasForeignKey(ds => ds.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
