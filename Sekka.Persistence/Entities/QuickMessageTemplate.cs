using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class QuickMessageTemplate : BaseEntity<Guid>
{
    public Guid? DriverId { get; set; }
    public string MessageText { get; set; } = null!;
    public MessageCategory Category { get; set; }
    public int UsageCount { get; set; }
    public bool IsSystemTemplate { get; set; }
    public int SortOrder { get; set; }

    public Driver? Driver { get; set; }
}
