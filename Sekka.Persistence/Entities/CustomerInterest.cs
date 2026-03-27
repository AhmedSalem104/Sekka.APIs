using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CustomerInterest : AuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Score { get; set; }
    public int SignalCount { get; set; }
    public DateTime? LastSignalAt { get; set; }
    public TrendDirection TrendDirection { get; set; }
    public decimal ConfidenceLevel { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public InterestCategory Category { get; set; } = null!;
}
