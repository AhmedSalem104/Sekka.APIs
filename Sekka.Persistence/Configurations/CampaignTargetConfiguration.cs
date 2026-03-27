using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class CampaignTargetConfiguration : IEntityTypeConfiguration<CampaignTarget>
{
    public void Configure(EntityTypeBuilder<CampaignTarget> builder)
    {
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.CampaignType);
        builder.HasIndex(c => c.ScheduledAt);

        builder.Property(c => c.Name).HasMaxLength(200);
        builder.Property(c => c.MessageTemplate).HasMaxLength(1000);

        builder.HasOne(c => c.Segment)
            .WithMany()
            .HasForeignKey(c => c.SegmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
