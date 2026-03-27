using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class BehaviorPatternConfiguration : IEntityTypeConfiguration<BehaviorPattern>
{
    public void Configure(EntityTypeBuilder<BehaviorPattern> builder)
    {
        builder.HasIndex(b => new { b.CustomerId, b.DriverId, b.PatternType, b.PatternKey }).IsUnique();
        builder.HasIndex(b => b.CustomerId);
        builder.HasIndex(b => b.DriverId);

        builder.Property(b => b.PatternKey).HasMaxLength(100);
        builder.Property(b => b.PatternValue).HasMaxLength(500);
        builder.Property(b => b.Confidence).HasPrecision(3, 2);

        builder.HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Driver)
            .WithMany(d => d.BehaviorPatterns)
            .HasForeignKey(b => b.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
