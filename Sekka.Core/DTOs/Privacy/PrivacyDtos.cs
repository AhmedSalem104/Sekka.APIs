using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Privacy;

public class ConsentDto
{
    public string ConsentType { get; set; } = null!;
    public bool IsGranted { get; set; }
    public DateTime? GrantedAt { get; set; }
    public string Description { get; set; } = null!;
}

public class UpdateConsentDto
{
    public bool IsGranted { get; set; }
}

public class DataExportDto
{
    public Guid RequestId { get; set; }
    public string Status { get; set; } = null!;
    public string? DownloadUrl { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class DeletionRequestDto
{
    public string RequestType { get; set; } = null!;
    public string? Reason { get; set; }
    public DeletionRequestStatus? Status { get; set; }
}
