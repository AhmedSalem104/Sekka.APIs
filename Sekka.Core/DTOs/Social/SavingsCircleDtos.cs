using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Social;

public class CreateCircleDto
{
    public string Name { get; set; } = null!;
    public decimal MonthlyAmount { get; set; }
    public int MaxMembers { get; set; }
    public int DurationMonths { get; set; }
    public int MinHealthScore { get; set; } = 80;
}

public class CircleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal MonthlyAmount { get; set; }
    public int MaxMembers { get; set; }
    public int CurrentMembersCount { get; set; }
    public int DurationMonths { get; set; }
    public int CurrentRound { get; set; }
    public CircleStatus Status { get; set; }
    public int MinHealthScore { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CircleDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal MonthlyAmount { get; set; }
    public int MaxMembers { get; set; }
    public int DurationMonths { get; set; }
    public int CurrentRound { get; set; }
    public CircleStatus Status { get; set; }
    public int MinHealthScore { get; set; }
    public DateTime? StartDate { get; set; }
    public Guid CreatorDriverId { get; set; }
    public List<CircleMemberDto> Members { get; set; } = new();
    public List<CirclePaymentDto> RecentPayments { get; set; } = new();
}

public class CircleMemberDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public int TurnOrder { get; set; }
    public CircleMemberStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class CirclePaymentDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = null!;
    public int RoundNumber { get; set; }
    public decimal Amount { get; set; }
    public CirclePaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
}
