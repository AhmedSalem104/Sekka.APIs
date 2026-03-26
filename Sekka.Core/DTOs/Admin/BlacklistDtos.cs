namespace Sekka.Core.DTOs.Admin;

public class BlacklistEntryDto
{
    public string PhoneNumber { get; set; } = null!;
    public int ReportCount { get; set; }
    public decimal SeverityScore { get; set; }
    public DateTime LastReportedAt { get; set; }
    public bool IsVerified { get; set; }
}

public class BlacklistReportDto
{
    public string PhoneNumber { get; set; } = null!;
    public int ReportCount { get; set; }
    public decimal SeverityScore { get; set; }
    public DateTime LastReportedAt { get; set; }
    public string? ReporterDriverName { get; set; }
}
