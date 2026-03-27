using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.NameEn).HasMaxLength(100);

        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.ParentRegionId);

        builder.HasOne(r => r.ParentRegion)
            .WithMany(r => r.Children)
            .HasForeignKey(r => r.ParentRegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
