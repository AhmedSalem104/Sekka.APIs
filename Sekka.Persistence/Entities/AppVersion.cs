using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class AppVersion : BaseEntity<Guid>
{
    public AppPlatform Platform { get; set; }
    public string VersionCode { get; set; } = null!;
    public int VersionNumber { get; set; }
    public string MinRequiredVersion { get; set; } = null!;
    public int MinRequiredVersionNumber { get; set; }
    public string StoreUrl { get; set; } = null!;
    public string? ReleaseNotes { get; set; }
    public string? ReleaseNotesEn { get; set; }
    public bool IsForceUpdate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ReleasedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
