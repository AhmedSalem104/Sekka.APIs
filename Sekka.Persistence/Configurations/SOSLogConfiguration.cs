using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class SOSLogConfiguration : IEntityTypeConfiguration<SOSLog>
{
    public void Configure(EntityTypeBuilder<SOSLog> builder)
    {
        builder.HasIndex(s => s.DriverId);
        builder.HasIndex(s => s.Status);

        builder.Property(s => s.Notes).HasMaxLength(500);
        builder.Property(s => s.AcknowledgedBy).HasMaxLength(100);
        builder.Property(s => s.ResolvedBy).HasMaxLength(100);

        builder.HasOne(s => s.Driver)
            .WithMany()
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
