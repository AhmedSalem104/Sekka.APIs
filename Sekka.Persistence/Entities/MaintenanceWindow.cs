using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class MaintenanceWindow : BaseEntity<Guid>
{
    public string Title { get; set; } = null!;
    public string? TitleEn { get; set; }
    public string Message { get; set; } = null!;
    public string? MessageEn { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFullBlock { get; set; }
    public string? AffectedServices { get; set; } // JSON
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
