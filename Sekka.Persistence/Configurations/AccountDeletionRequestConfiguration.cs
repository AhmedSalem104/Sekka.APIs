using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sekka.Persistence.Configurations;

public class AccountDeletionRequestConfiguration : IEntityTypeConfiguration<Entities.AccountDeletionRequest>
{
    public void Configure(EntityTypeBuilder<Entities.AccountDeletionRequest> builder)
    {
        builder.Property(r => r.Reason).HasMaxLength(500);
        builder.Property(r => r.ConfirmationCode).HasMaxLength(10);
        builder.Property(r => r.ProcessedBy).HasMaxLength(100);
        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.Status);
    }
}
