namespace Sekka.Core.DTOs.Financial;

public class SurgeMultiplierDto
{
    public Guid? RegionId { get; set; }
    public decimal Multiplier { get; set; }
    public string? RuleName { get; set; }
    public DateTime CalculatedAt { get; set; }
}
