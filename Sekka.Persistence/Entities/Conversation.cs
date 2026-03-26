using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Conversation : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid? AdminUserId { get; set; }
    public ChatType ChatType { get; set; }
    public string? Subject { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Driver Driver { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
