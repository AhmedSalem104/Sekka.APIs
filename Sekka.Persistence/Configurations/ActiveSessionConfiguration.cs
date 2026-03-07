using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class ActiveSessionConfiguration : IEntityTypeConfiguration<Entities.ActiveSession>
{
    public void Configure(EntityTypeBuilder<Entities.ActiveSession> builder)
    {
        builder.Property(s => s.DeviceName).HasMaxLength(200);
        builder.Property(s => s.IpAddress).HasMaxLength(45);
        builder.Property(s => s.RefreshToken).IsRequired().HasMaxLength(500);
        builder.HasIndex(s => s.DriverId);
    }
}
