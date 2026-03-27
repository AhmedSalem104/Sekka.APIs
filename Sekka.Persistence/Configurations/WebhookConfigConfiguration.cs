using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class WebhookConfigConfiguration : IEntityTypeConfiguration<WebhookConfig>
{
    public void Configure(EntityTypeBuilder<WebhookConfig> builder)
    {
        builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
        builder.Property(w => w.Url).IsRequired().HasMaxLength(500);
        builder.Property(w => w.Secret).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Events).IsRequired();

        builder.HasIndex(w => w.IsActive);
        builder.HasIndex(w => w.DriverId);
        builder.HasIndex(w => w.PartnerId);

        builder.HasOne(w => w.Driver)
            .WithMany()
            .HasForeignKey(w => w.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Partner)
            .WithMany()
            .HasForeignKey(w => w.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
