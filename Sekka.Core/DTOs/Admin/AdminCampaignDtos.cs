using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Admin;

public class AdminCampaignDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CampaignType { get; set; } = null!;
    public string? SegmentName { get; set; }
    public string? CategoryName { get; set; }
    public string Channel { get; set; } = null!;
    public int TargetCount { get; set; }
    public int SentCount { get; set; }
    public int OpenCount { get; set; }
    public int ConversionCount { get; set; }
    public decimal ConversionRate { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminCampaignDetailDto : AdminCampaignDto
{
    public string MessageTemplate { get; set; } = null!;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedByName { get; set; } = null!;
}

public class CreateCampaignDto
{
    public string Name { get; set; } = null!;
    public string CampaignType { get; set; } = null!;
    public Guid? SegmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public string MessageTemplate { get; set; } = null!;
    public string Channel { get; set; } = null!;
    public DateTime? ScheduledAt { get; set; }
}

public class UpdateCampaignDto
{
    public string? Name { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Channel { get; set; }
    public DateTime? ScheduledAt { get; set; }
}

public class CampaignFilterDto : PaginationDto
{
    public string? CampaignType { get; set; }
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class CampaignStatsDto
{
    public int TotalCampaigns { get; set; }
    public int ActiveCampaigns { get; set; }
    public int CompletedCampaigns { get; set; }
    public int DraftCampaigns { get; set; }
    public decimal AverageConversionRate { get; set; }
    public int TotalMessagesSent { get; set; }
    public int TotalConversions { get; set; }
}

public class CampaignAnalyticsDto
{
    public int TotalSent { get; set; }
    public int TotalOpened { get; set; }
    public int TotalConversions { get; set; }
    public decimal OpenRate { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal RevenueGenerated { get; set; }
    public List<CampaignDailyStatDto> DailyStats { get; set; } = new();
}

public class CampaignDailyStatDto
{
    public DateOnly Date { get; set; }
    public int Sent { get; set; }
    public int Opened { get; set; }
    public int Conversions { get; set; }
}
