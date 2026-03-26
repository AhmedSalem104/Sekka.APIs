using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class OrderPhotoConfiguration : IEntityTypeConfiguration<OrderPhoto>
{
    public void Configure(EntityTypeBuilder<OrderPhoto> builder)
    {
        builder.HasIndex(p => p.OrderId);
        builder.Property(p => p.PhotoUrl).HasMaxLength(500);

        builder.HasOne(p => p.Order)
            .WithMany(o => o.Photos)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
