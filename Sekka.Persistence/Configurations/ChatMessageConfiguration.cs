using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasIndex(cm => cm.ConversationId);

        builder.Property(cm => cm.SenderType).IsRequired().HasMaxLength(20);
        builder.Property(cm => cm.Content).IsRequired().HasMaxLength(2000);
        builder.Property(cm => cm.AttachmentUrl).HasMaxLength(500);

        builder.HasOne(cm => cm.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(cm => cm.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
