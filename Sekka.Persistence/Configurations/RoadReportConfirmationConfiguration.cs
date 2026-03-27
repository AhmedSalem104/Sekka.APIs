using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class RoadReportConfirmationConfiguration : IEntityTypeConfiguration<RoadReportConfirmation>
{
    public void Configure(EntityTypeBuilder<RoadReportConfirmation> builder)
    {
        builder.HasIndex(c => new { c.ReportId, c.DriverId }).IsUnique();
        builder.HasOne(c => c.Report).WithMany(r => r.Confirmations).HasForeignKey(c => c.ReportId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.Driver).WithMany().HasForeignKey(c => c.DriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
