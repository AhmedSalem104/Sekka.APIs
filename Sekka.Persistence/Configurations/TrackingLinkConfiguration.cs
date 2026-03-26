using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class TrackingLinkConfiguration : IEntityTypeConfiguration<TrackingLink>
{
    public void Configure(EntityTypeBuilder<TrackingLink> builder)
    {
        builder.HasIndex(t => t.OrderId).IsUnique();
        builder.HasIndex(t => t.TrackingCode).IsUnique();
        builder.Property(t => t.TrackingCode).HasMaxLength(20);

        builder.HasOne(t => t.Order)
            .WithOne(o => o.TrackingLink)
            .HasForeignKey<TrackingLink>(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
