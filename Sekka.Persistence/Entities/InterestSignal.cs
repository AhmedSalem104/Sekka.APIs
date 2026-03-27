using Sekka.Core.Enums;

namespace Sekka.Persistence.Entities;

public class InterestSignal
{
    public long Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? OrderId { get; set; }
    public SignalType SignalType { get; set; }
    public decimal Weight { get; set; } = 1.0m;
    public decimal? Amount { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public InterestCategory Category { get; set; } = null!;
    public Order? Order { get; set; }
}
