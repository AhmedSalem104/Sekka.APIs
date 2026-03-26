using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class QuickMessageTemplateConfiguration : IEntityTypeConfiguration<QuickMessageTemplate>
{
    public void Configure(EntityTypeBuilder<QuickMessageTemplate> builder)
    {
        builder.HasIndex(qmt => qmt.DriverId);

        builder.Property(qmt => qmt.MessageText).IsRequired().HasMaxLength(500);

        builder.HasOne(qmt => qmt.Driver)
            .WithMany()
            .HasForeignKey(qmt => qmt.DriverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
