namespace Sekka.Core.DTOs.Communication;

public class ActivateSOSDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get; set; }
}

public class ResolveSOSDto
{
    public string? Resolution { get; set; }
    public bool WasFalseAlarm { get; set; }
}

public class SOSLogDto
{
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Status { get; set; } = null!;
    public List<string> NotifiedContacts { get; set; } = new();
    public bool AdminNotified { get; set; }
    public string? Notes { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
