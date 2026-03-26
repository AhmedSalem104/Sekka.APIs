using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class BreakLogConfiguration : IEntityTypeConfiguration<BreakLog>
{
    public void Configure(EntityTypeBuilder<BreakLog> builder)
    {
        builder.HasIndex(b => b.DriverId);

        builder.Property(b => b.LocationDescription).HasMaxLength(200);

        builder.HasOne(b => b.Driver)
            .WithMany(d => d.BreakLogs)
            .HasForeignKey(b => b.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
