using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class OrderSourceTag : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public SourceTagType SourceType { get; set; }
    public string? SourceName { get; set; }
    public string? SourceReference { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
