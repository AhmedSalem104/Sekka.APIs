using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CustomerRecommendation : BaseEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public Guid? RuleId { get; set; }
    public RecommendationType RecommendationType { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public Guid? CategoryId { get; set; }
    public decimal RelevanceScore { get; set; }
    public RecommendationStatus Status { get; set; }
    public bool IsRead { get; set; }
    public bool IsDismissed { get; set; }
    public bool IsActedUpon { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public RecommendationRule? Rule { get; set; }
    public InterestCategory? Category { get; set; }
}
