using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class CustomerInterestConfiguration : IEntityTypeConfiguration<CustomerInterest>
{
    public void Configure(EntityTypeBuilder<CustomerInterest> builder)
    {
        builder.HasIndex(ci => new { ci.CustomerId, ci.DriverId, ci.CategoryId }).IsUnique();
        builder.HasIndex(ci => ci.CustomerId);
        builder.HasIndex(ci => ci.DriverId);
        builder.HasIndex(ci => ci.CategoryId);

        builder.Property(ci => ci.Score).HasPrecision(5, 2);
        builder.Property(ci => ci.ConfidenceLevel).HasPrecision(3, 2);

        builder.HasOne(ci => ci.Customer)
            .WithMany()
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ci => ci.Driver)
            .WithMany(d => d.CustomerInterestsData)
            .HasForeignKey(ci => ci.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ci => ci.Category)
            .WithMany(c => c.CustomerInterests)
            .HasForeignKey(ci => ci.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
