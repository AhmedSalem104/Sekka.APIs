using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class EmergencyContact : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Relationship { get; set; }
    public int SortOrder { get; set; }

    public Driver Driver { get; set; } = null!;
}
