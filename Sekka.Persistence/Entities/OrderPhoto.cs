using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class OrderPhoto : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public string PhotoUrl { get; set; } = null!;
    public PhotoType PhotoType { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
}
