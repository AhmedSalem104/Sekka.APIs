using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class RecommendationRuleConfiguration : IEntityTypeConfiguration<RecommendationRule>
{
    public void Configure(EntityTypeBuilder<RecommendationRule> builder)
    {
        builder.HasIndex(r => r.RecommendationType);
        builder.HasIndex(r => r.IsActive);

        builder.Property(r => r.Name).HasMaxLength(100);
        builder.Property(r => r.MinInterestScore).HasPrecision(5, 2);
        builder.Property(r => r.MessageTemplate).HasMaxLength(500);
        builder.Property(r => r.MessageTemplateAr).HasMaxLength(500);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
