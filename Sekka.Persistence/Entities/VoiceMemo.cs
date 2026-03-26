using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class VoiceMemo : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public string AudioUrl { get; set; } = null!;
    public string? Transcription { get; set; }
    public int? DurationSeconds { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Order? Order { get; set; }
}
