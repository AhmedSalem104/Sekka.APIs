using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class SystemNoticeConfiguration : IEntityTypeConfiguration<SystemNotice>
{
    public void Configure(EntityTypeBuilder<SystemNotice> builder)
    {
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.TitleEn).HasMaxLength(200);
        builder.Property(n => n.Body).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.BodyEn).HasMaxLength(1000);
        builder.Property(n => n.ActionUrl).HasMaxLength(500);
        builder.Property(n => n.ActionLabel).HasMaxLength(100);
        builder.Property(n => n.BackgroundColor).HasMaxLength(10);
        builder.Property(n => n.IconUrl).HasMaxLength(500);
        builder.Property(n => n.CreatedBy).HasMaxLength(100);

        builder.HasIndex(n => n.NoticeType);
        builder.HasIndex(n => n.TargetAudience);
        builder.HasIndex(n => n.IsActive);
        builder.HasIndex(n => n.StartsAt);
        builder.HasIndex(n => n.ExpiresAt);
        builder.HasIndex(n => n.Priority);

        builder.HasOne(n => n.TargetRegion)
            .WithMany()
            .HasForeignKey(n => n.TargetRegionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
