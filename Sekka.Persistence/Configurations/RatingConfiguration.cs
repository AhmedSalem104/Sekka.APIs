using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasIndex(r => new { r.DriverId, r.CustomerId });
        builder.HasIndex(r => r.OrderId);
        builder.Property(r => r.FeedbackText).HasMaxLength(500);
        builder.HasOne(r => r.Driver).WithMany(d => d.Ratings).HasForeignKey(r => r.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Customer).WithMany(c => c.Ratings).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(r => r.Order).WithMany().HasForeignKey(r => r.OrderId).OnDelete(DeleteBehavior.SetNull);
    }
}
