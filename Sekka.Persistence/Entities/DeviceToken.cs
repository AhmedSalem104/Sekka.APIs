using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DeviceToken : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string Token { get; set; } = null!;
    public DevicePlatform Platform { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UpdatedAt { get; set; }

    public Driver Driver { get; set; } = null!;
}
