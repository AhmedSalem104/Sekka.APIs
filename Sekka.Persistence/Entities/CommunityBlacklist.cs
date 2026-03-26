namespace Sekka.Persistence.Entities;

public class CommunityBlacklist
{
    public string PhoneNumber { get; set; } = null!;
    public int ReportCount { get; set; } = 1;
    public decimal SeverityScore { get; set; }
    public DateTime LastReportedAt { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; }
}
