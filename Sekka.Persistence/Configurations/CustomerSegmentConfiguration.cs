using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class CustomerSegmentConfiguration : IEntityTypeConfiguration<CustomerSegment>
{
    public void Configure(EntityTypeBuilder<CustomerSegment> builder)
    {
        builder.HasIndex(s => s.Name).IsUnique();
        builder.HasIndex(s => s.SegmentType);

        builder.Property(s => s.Name).HasMaxLength(100);
        builder.Property(s => s.NameAr).HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.ColorHex).HasMaxLength(7);
        builder.Property(s => s.MinScore).HasPrecision(5, 2);
        builder.Property(s => s.MaxScore).HasPrecision(5, 2);
    }
}
