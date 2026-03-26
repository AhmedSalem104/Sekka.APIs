using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class SyncQueueConfiguration : IEntityTypeConfiguration<SyncQueue>
{
    public void Configure(EntityTypeBuilder<SyncQueue> builder)
    {
        builder.HasIndex(s => new { s.DriverId, s.Status });
        builder.Property(s => s.EntityType).HasMaxLength(50);
        builder.Property(s => s.EntityId).HasMaxLength(50);
        builder.Property(s => s.ConflictResolution).HasMaxLength(500);

        builder.HasOne(s => s.Driver)
            .WithMany(d => d.SyncQueues)
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
