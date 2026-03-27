using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class DriverAchievementConfiguration : IEntityTypeConfiguration<DriverAchievement>
{
    public void Configure(EntityTypeBuilder<DriverAchievement> builder)
    {
        builder.HasIndex(a => new { a.DriverId, a.ChallengeId }).IsUnique();
        builder.Property(a => a.CurrentProgress).HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(a => a.IsCompleted).HasDefaultValue(false);
        builder.Property(a => a.PointsEarned).HasDefaultValue(0);
        builder.HasOne(a => a.Driver).WithMany(d => d.Achievements).HasForeignKey(a => a.DriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Challenge).WithMany(c => c.DriverAchievements).HasForeignKey(a => a.ChallengeId).OnDelete(DeleteBehavior.Cascade);
    }
}
