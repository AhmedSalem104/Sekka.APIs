using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class AccountDeletionRequest : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string? Reason { get; set; }
    public DeletionRequestStatus Status { get; set; }
    public string? ConfirmationCode { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? ProcessedBy { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
