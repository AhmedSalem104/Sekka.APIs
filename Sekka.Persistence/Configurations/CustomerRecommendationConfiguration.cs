using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class CustomerRecommendationConfiguration : IEntityTypeConfiguration<CustomerRecommendation>
{
    public void Configure(EntityTypeBuilder<CustomerRecommendation> builder)
    {
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.RecommendationType);

        builder.Property(r => r.Title).HasMaxLength(200);
        builder.Property(r => r.Message).HasMaxLength(500);
        builder.Property(r => r.RelevanceScore).HasPrecision(5, 2);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Driver)
            .WithMany()
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Rule)
            .WithMany()
            .HasForeignKey(r => r.RuleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
