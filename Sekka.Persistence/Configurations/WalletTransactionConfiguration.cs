using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasIndex(wt => wt.DriverId);
        builder.HasIndex(wt => wt.OrderId);
        builder.HasIndex(wt => wt.SettlementId);

        builder.Property(wt => wt.Amount).HasPrecision(18, 2);
        builder.Property(wt => wt.BalanceAfter).HasPrecision(18, 2);
        builder.Property(wt => wt.Description).HasMaxLength(500);

        builder.HasOne(wt => wt.Driver)
            .WithMany(d => d.WalletTransactions)
            .HasForeignKey(wt => wt.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wt => wt.Order)
            .WithMany()
            .HasForeignKey(wt => wt.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(wt => wt.Settlement)
            .WithMany(s => s.WalletTransactions)
            .HasForeignKey(wt => wt.SettlementId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
