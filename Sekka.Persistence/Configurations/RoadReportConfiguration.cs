using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class RoadReportConfiguration : IEntityTypeConfiguration<RoadReport>
{
    public void Configure(EntityTypeBuilder<RoadReport> builder)
    {
        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.Type);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.ExpiresAt);
        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.ConfirmationsCount).HasDefaultValue(0);
        builder.Property(r => r.IsActive).HasDefaultValue(true);
        builder.HasOne(r => r.Driver).WithMany(d => d.RoadReports).HasForeignKey(r => r.DriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
