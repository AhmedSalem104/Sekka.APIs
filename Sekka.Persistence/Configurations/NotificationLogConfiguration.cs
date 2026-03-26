using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedOnAdd();

        builder.HasIndex(n => n.RecipientId);
        builder.HasIndex(n => n.Channel);

        builder.Property(n => n.TemplateName).HasMaxLength(100);
        builder.Property(n => n.Subject).HasMaxLength(200);
        builder.Property(n => n.ErrorMessage).HasMaxLength(500);
        builder.Property(n => n.ExternalId).HasMaxLength(200);
    }
}
