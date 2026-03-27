using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SavingsCirclePayment : BaseEntity<Guid>
{
    public Guid CircleId { get; set; }
    public Guid MemberId { get; set; }
    public int RoundNumber { get; set; }
    public decimal Amount { get; set; }
    public CirclePaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navigation
    public SavingsCircle Circle { get; set; } = null!;
    public SavingsCircleMember Member { get; set; } = null!;
}
