using System.ComponentModel.DataAnnotations;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Social;

public class CreateRoadReportDto
{
    public RoadReportType Type { get; set; }

    [Range(-90, 90, ErrorMessage = "خط العرض لازم يكون بين -90 و 90")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "خط الطول لازم يكون بين -180 و 180")]
    public double Longitude { get; set; }

    public string? Description { get; set; }
    public ReportSeverity Severity { get; set; } = ReportSeverity.Medium;
}

public class RoadReportDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public RoadReportType Type { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }
    public ReportSeverity Severity { get; set; }
    public int ConfirmationsCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
