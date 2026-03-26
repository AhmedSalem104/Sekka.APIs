using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasIndex(c => c.DriverId);

        builder.Property(c => c.Subject).HasMaxLength(200);

        builder.HasOne(c => c.Driver)
            .WithMany()
            .HasForeignKey(c => c.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
