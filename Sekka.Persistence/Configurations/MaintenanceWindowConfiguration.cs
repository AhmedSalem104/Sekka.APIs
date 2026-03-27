using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class MaintenanceWindowConfiguration : IEntityTypeConfiguration<MaintenanceWindow>
{
    public void Configure(EntityTypeBuilder<MaintenanceWindow> builder)
    {
        builder.Property(m => m.Title).IsRequired().HasMaxLength(200);
        builder.Property(m => m.TitleEn).HasMaxLength(200);
        builder.Property(m => m.Message).IsRequired().HasMaxLength(1000);
        builder.Property(m => m.MessageEn).HasMaxLength(1000);
        builder.Property(m => m.CreatedBy).HasMaxLength(100);

        builder.HasIndex(m => m.IsActive);
        builder.HasIndex(m => m.StartTime);
        builder.HasIndex(m => m.EndTime);
    }
}
