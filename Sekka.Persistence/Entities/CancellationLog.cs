using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CancellationLog : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public CancellationReason CancellationReason { get; set; }
    public string? ReasonText { get; set; }
    public decimal LossAmount { get; set; }
    public double? DistanceTravelledKm { get; set; }
    public int? TimeLostMinutes { get; set; }
    public decimal? FuelCostLost { get; set; }
    public string? DocumentationUrl { get; set; }
    public Guid? TransferredToDriverId { get; set; }
    public DateTime CancelledAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
    public Driver? TransferredToDriver { get; set; }
}
