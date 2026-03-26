using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Vehicle;

public class StartBreakDto
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationDescription { get; set; }
    public int? EnergyBefore { get; set; }
}

public class EndBreakDto
{
    public int? EnergyAfter { get; set; }
}

public class BreakLogDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string? LocationDescription { get; set; }
    public int? EnergyBefore { get; set; }
    public int? EnergyAfter { get; set; }
}

public class BreakSuggestionDto
{
    public bool ShouldBreak { get; set; }
    public BreakUrgency Urgency { get; set; }
    public int SuggestedDurationMinutes { get; set; }
    public string Reason { get; set; } = null!;
    public List<ParkingSpotDto> NearbySpots { get; set; } = new();
}
