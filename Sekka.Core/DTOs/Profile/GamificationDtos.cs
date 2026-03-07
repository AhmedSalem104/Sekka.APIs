using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Profile;

public class ChallengeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ChallengeType ChallengeType { get; set; }
    public decimal TargetValue { get; set; }
    public decimal CurrentProgress { get; set; }
    public decimal ProgressPercentage { get; set; }
    public int RewardPoints { get; set; }
    public string? BadgeName { get; set; }
    public bool IsCompleted { get; set; }
}

public class DriverAchievementDto
{
    public Guid Id { get; set; }
    public string ChallengeName { get; set; } = null!;
    public string? BadgeName { get; set; }
    public string? BadgeIconUrl { get; set; }
    public int PointsEarned { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class LeaderboardDto
{
    public int MyRank { get; set; }
    public int MyPoints { get; set; }
    public List<LeaderboardEntryDto> TopDrivers { get; set; } = new();
}

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string DriverName { get; set; } = null!;
    public int Points { get; set; }
    public int Level { get; set; }
    public int OrdersThisMonth { get; set; }
}
