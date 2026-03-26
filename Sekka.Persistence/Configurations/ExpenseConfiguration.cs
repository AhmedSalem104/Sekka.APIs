using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasIndex(e => e.DriverId);

        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.ReceiptImageUrl).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(500);

        builder.HasOne(e => e.Driver)
            .WithMany(d => d.Expenses)
            .HasForeignKey(e => e.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
