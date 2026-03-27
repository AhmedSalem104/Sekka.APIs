using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class SavingsCircleMemberConfiguration : IEntityTypeConfiguration<SavingsCircleMember>
{
    public void Configure(EntityTypeBuilder<SavingsCircleMember> builder)
    {
        builder.HasIndex(m => new { m.CircleId, m.DriverId }).IsUnique();
        builder.HasIndex(m => m.DriverId);
        builder.HasOne(m => m.Circle).WithMany(c => c.Members).HasForeignKey(m => m.CircleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.Driver).WithMany(d => d.SavingsCircleMemberships).HasForeignKey(m => m.DriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
