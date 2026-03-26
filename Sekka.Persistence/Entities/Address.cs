using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Address : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid? CustomerId { get; set; }
    public string AddressText { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public AddressType AddressType { get; set; }
    public int VisitCount { get; set; }
    public string? Landmarks { get; set; }
    public string? DeliveryNotes { get; set; }
    public DateTime? LastVisitedAt { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Customer? Customer { get; set; }
}
