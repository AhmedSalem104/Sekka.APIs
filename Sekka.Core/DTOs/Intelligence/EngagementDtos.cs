namespace Sekka.Core.DTOs.Intelligence;

public class CustomerEngagementDto
{
    public Guid CustomerId { get; set; }
    public string EngagementLevel { get; set; } = null!;
    public decimal EngagementScore { get; set; }
    public DateTime? LastInteraction { get; set; }
    public int InteractionCount30Days { get; set; }
    public bool IsAtRisk { get; set; }
}
