using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasIndex(n => n.DriverId);
        builder.HasIndex(n => n.IsRead);

        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.ActionType).HasMaxLength(50);
        builder.Property(n => n.ActionData).HasMaxLength(500);

        builder.HasOne(n => n.Driver)
            .WithMany()
            .HasForeignKey(n => n.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
