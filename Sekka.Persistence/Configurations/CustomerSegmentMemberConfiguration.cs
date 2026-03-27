using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class CustomerSegmentMemberConfiguration : IEntityTypeConfiguration<CustomerSegmentMember>
{
    public void Configure(EntityTypeBuilder<CustomerSegmentMember> builder)
    {
        builder.HasIndex(m => new { m.SegmentId, m.CustomerId, m.DriverId }).IsUnique();
        builder.HasIndex(m => m.SegmentId);
        builder.HasIndex(m => m.CustomerId);
        builder.HasIndex(m => m.DriverId);

        builder.Property(m => m.Score).HasPrecision(5, 2);

        builder.HasOne(m => m.Segment)
            .WithMany(s => s.Members)
            .HasForeignKey(m => m.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Customer)
            .WithMany()
            .HasForeignKey(m => m.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Driver)
            .WithMany()
            .HasForeignKey(m => m.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
