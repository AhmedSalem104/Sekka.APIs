using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;
namespace Sekka.Persistence.Configurations;
public class FieldAssistanceRequestConfiguration : IEntityTypeConfiguration<FieldAssistanceRequest>
{
    public void Configure(EntityTypeBuilder<FieldAssistanceRequest> builder)
    {
        builder.HasIndex(r => r.RequestingDriverId);
        builder.HasIndex(r => r.AssistingDriverId);
        builder.HasIndex(r => r.Status);
        builder.Property(r => r.Message).HasMaxLength(500);
        builder.HasOne(r => r.RequestingDriver).WithMany(d => d.FieldAssistanceRequestsSent).HasForeignKey(r => r.RequestingDriverId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.AssistingDriver).WithMany().HasForeignKey(r => r.AssistingDriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
