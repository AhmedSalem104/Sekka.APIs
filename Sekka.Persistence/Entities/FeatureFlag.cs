using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class FeatureFlag : BaseEntity<Guid>
{
    public string FeatureKey { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? DisplayNameEn { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public bool EnabledForPremiumOnly { get; set; }
    public string? EnabledForUserIds { get; set; } // JSON
    public int EnabledForPercentage { get; set; } = 100;
    public string? MinAppVersion { get; set; }
    public string? Category { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
