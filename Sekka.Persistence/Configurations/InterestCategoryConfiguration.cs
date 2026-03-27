using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class InterestCategoryConfiguration : IEntityTypeConfiguration<InterestCategory>
{
    public void Configure(EntityTypeBuilder<InterestCategory> builder)
    {
        builder.HasIndex(c => c.ParentCategoryId);
        builder.HasIndex(c => c.SortOrder);

        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.NameAr).HasMaxLength(100);
        builder.Property(c => c.IconUrl).HasMaxLength(500);
        builder.Property(c => c.ColorHex).HasMaxLength(7);

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
