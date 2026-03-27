using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class BehaviorPattern : BaseEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public BehaviorPatternType PatternType { get; set; }
    public string PatternKey { get; set; } = null!;
    public string PatternValue { get; set; } = null!;
    public string? PatternData { get; set; }
    public int Occurrences { get; set; } = 1;
    public decimal Confidence { get; set; }
    public DateTime FirstDetectedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastDetectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
