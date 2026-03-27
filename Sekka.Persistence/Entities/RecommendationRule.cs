using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class RecommendationRule : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public RecommendationType RecommendationType { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal MinInterestScore { get; set; }
    public string? Condition { get; set; }
    public string MessageTemplate { get; set; } = null!;
    public string MessageTemplateAr { get; set; } = null!;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public InterestCategory? Category { get; set; }
}
