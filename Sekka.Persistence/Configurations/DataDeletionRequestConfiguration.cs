using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class DataDeletionRequestConfiguration : IEntityTypeConfiguration<Entities.DataDeletionRequest>
{
    public void Configure(EntityTypeBuilder<Entities.DataDeletionRequest> builder)
    {
        builder.Property(r => r.RequestType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.DataExportUrl).HasMaxLength(500);
        builder.Property(r => r.ProcessedBy).HasMaxLength(100);
        builder.Property(r => r.Notes).HasMaxLength(500);
        builder.HasIndex(r => r.DriverId);
    }
}
