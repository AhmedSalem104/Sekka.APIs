using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class ActiveSession : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string? DeviceName { get; set; }
    public DevicePlatform DevicePlatform { get; set; }
    public string? IpAddress { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    public bool IsCurrentSession { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
