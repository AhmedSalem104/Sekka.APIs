using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.HasIndex(dt => dt.DriverId);

        builder.Property(dt => dt.Token).IsRequired().HasMaxLength(500);

        builder.HasOne(dt => dt.Driver)
            .WithMany()
            .HasForeignKey(dt => dt.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
