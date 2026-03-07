using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class UserConsentConfiguration : IEntityTypeConfiguration<Entities.UserConsent>
{
    public void Configure(EntityTypeBuilder<Entities.UserConsent> builder)
    {
        builder.Property(c => c.ConsentType).IsRequired().HasMaxLength(100);
        builder.Property(c => c.IpAddress).HasMaxLength(45);
        builder.HasIndex(c => new { c.DriverId, c.ConsentType });
    }
}
