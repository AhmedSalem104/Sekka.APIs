using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class FieldAssistanceRequest : BaseEntity<Guid>
{
    public Guid RequestingDriverId { get; set; }
    public Guid? AssistingDriverId { get; set; }
    public AssistanceType Type { get; set; }
    public string Title { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Message { get; set; }
    public AssistanceStatus Status { get; set; }
    public Guid? OrderId { get; set; }
    public string? DriverPhone { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // Navigation
    public Driver RequestingDriver { get; set; } = null!;
    public Driver? AssistingDriver { get; set; }
    public Order? Order { get; set; }
}
