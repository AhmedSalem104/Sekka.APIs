using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class WaitingTimerConfiguration : IEntityTypeConfiguration<WaitingTimer>
{
    public void Configure(EntityTypeBuilder<WaitingTimer> builder)
    {
        builder.HasIndex(w => w.OrderId);
        builder.Property(w => w.Reason).HasMaxLength(200);

        builder.HasOne(w => w.Order)
            .WithMany(o => o.WaitingTimers)
            .HasForeignKey(w => w.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
