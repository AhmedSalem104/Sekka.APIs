using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Admin;

public class AdminSOSFilterDto : PaginationDto
{
    public string? Status { get; set; }
    public string? EscalationLevel { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class AdminSOSDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Status { get; set; } = null!;
    public string EscalationLevel { get; set; } = null!;
    public bool AdminAcknowledged { get; set; }
    public DateTime ActivatedAt { get; set; }
    public double? DurationMinutes { get; set; }
}

public class AdminSOSDetailDto : AdminSOSDto
{
    public List<string> NotifiedContacts { get; set; } = new();
    public bool LocationSharedWithFamily { get; set; }
    public List<EscalationEventDto> EscalationHistory { get; set; } = new();
}

public class EscalationEventDto
{
    public string Level { get; set; } = null!;
    public string EscalatedBy { get; set; } = null!;
    public string? Reason { get; set; }
    public DateTime Timestamp { get; set; }
}

public class EscalateSOSDto
{
    public string EscalationLevel { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public bool NotifyPolice { get; set; }
}

public class AdminResolveSOSDto
{
    public string Resolution { get; set; } = null!;
    public bool WasFalseAlarm { get; set; }
}

public class SOSStatsDto
{
    public int TotalSOSAlerts { get; set; }
    public int ActiveAlerts { get; set; }
    public int FalseAlarms { get; set; }
    public double AvgResponseTimeMinutes { get; set; }
    public int EscalatedCount { get; set; }
}

public class SOSResponseTimeDto
{
    public double Avg { get; set; }
    public double Fastest { get; set; }
    public double Slowest { get; set; }
    public int UnderFive { get; set; }
    public int OverFifteen { get; set; }
}

public class SOSHeatmapDto
{
    public List<SOSHeatmapPointDto> Points { get; set; } = new();
    public List<string> HighRiskAreas { get; set; } = new();
}

public class SOSHeatmapPointDto
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int AlertCount { get; set; }
}
