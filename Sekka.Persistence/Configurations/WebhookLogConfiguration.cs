using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class WebhookLogConfiguration : IEntityTypeConfiguration<WebhookLog>
{
    public void Configure(EntityTypeBuilder<WebhookLog> builder)
    {
        builder.Property(l => l.EventType).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Payload).IsRequired();
        builder.Property(l => l.SentAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(l => l.WebhookConfigId);
        builder.HasIndex(l => l.EventType);
        builder.HasIndex(l => l.IsSuccess);
        builder.HasIndex(l => l.SentAt);

        builder.HasOne(l => l.WebhookConfig)
            .WithMany()
            .HasForeignKey(l => l.WebhookConfigId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
