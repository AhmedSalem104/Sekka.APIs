using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class InterestSignalConfiguration : IEntityTypeConfiguration<InterestSignal>
{
    public void Configure(EntityTypeBuilder<InterestSignal> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.DriverId);
        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => s.IsProcessed);
        builder.HasIndex(s => s.CreatedAt);

        builder.Property(s => s.Weight).HasPrecision(3, 2);
        builder.Property(s => s.Amount).HasPrecision(18, 2);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Driver)
            .WithMany()
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Order)
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
