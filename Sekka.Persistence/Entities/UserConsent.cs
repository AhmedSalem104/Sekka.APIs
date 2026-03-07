using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class UserConsent : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string ConsentType { get; set; } = null!;
    public bool IsGranted { get; set; }
    public DateTime? GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? IpAddress { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
