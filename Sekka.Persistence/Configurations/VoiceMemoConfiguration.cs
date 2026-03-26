using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class VoiceMemoConfiguration : IEntityTypeConfiguration<VoiceMemo>
{
    public void Configure(EntityTypeBuilder<VoiceMemo> builder)
    {
        builder.HasIndex(v => v.DriverId);
        builder.HasIndex(v => v.OrderId);
        builder.Property(v => v.AudioUrl).HasMaxLength(500);
        builder.Property(v => v.Transcription).HasMaxLength(1000);

        builder.HasOne(v => v.Driver)
            .WithMany(d => d.VoiceMemos)
            .HasForeignKey(v => v.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Order)
            .WithMany(o => o.VoiceMemos)
            .HasForeignKey(v => v.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
