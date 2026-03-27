using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SavingsCircle : BaseEntity<Guid>
{
    public Guid CreatorDriverId { get; set; }
    public string Name { get; set; } = null!;
    public decimal MonthlyAmount { get; set; }
    public int MaxMembers { get; set; }
    public int DurationMonths { get; set; }
    public int CurrentRound { get; set; }
    public CircleStatus Status { get; set; }
    public int MinHealthScore { get; set; } = 80;
    public DateTime? StartDate { get; set; }

    // Navigation
    public Driver Creator { get; set; } = null!;
    public ICollection<SavingsCircleMember> Members { get; set; } = new List<SavingsCircleMember>();
    public ICollection<SavingsCirclePayment> Payments { get; set; } = new List<SavingsCirclePayment>();
}
