using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Admin;

public class AdminSegmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string SegmentType { get; set; } = null!;
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public bool IsAutomatic { get; set; }
    public int MemberCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastRefreshedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminSegmentDetailDto : AdminSegmentDto
{
    public string? Rules { get; set; }
    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }
    public List<string> TopInterests { get; set; } = new();
    public Dictionary<string, int> EngagementDistribution { get; set; } = new();
}

public class CreateSegmentDto
{
    public string Name { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string SegmentType { get; set; } = null!;
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public string? Rules { get; set; }
    public bool IsAutomatic { get; set; }
    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }
}

public class UpdateSegmentDto
{
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public string? SegmentType { get; set; }
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public string? Rules { get; set; }
    public bool? IsAutomatic { get; set; }
    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }
}

public class SegmentFilterDto : PaginationDto
{
    public string? SegmentType { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
}

public class SegmentMemberDto
{
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string CustomerPhone { get; set; } = null!;
    public string DriverName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public decimal? Score { get; set; }
    public int TotalOrders { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public string EngagementLevel { get; set; } = null!;
}

public class SegmentAnalyticsDto
{
    public int TotalSegments { get; set; }
    public int AutomaticSegments { get; set; }
    public int ManualSegments { get; set; }
    public int TotalMembers { get; set; }
    public decimal AverageMembersPerSegment { get; set; }
    public List<SegmentGrowthDto> GrowthTrend { get; set; } = new();
    public List<SegmentDistributionItemDto> Distribution { get; set; } = new();
}

public class SegmentGrowthDto
{
    public DateOnly Date { get; set; }
    public int NewMembers { get; set; }
    public int RemovedMembers { get; set; }
    public int NetGrowth { get; set; }
}

public class SegmentDistributionDto
{
    public List<SegmentDistributionItemDto> Items { get; set; } = new();
}

public class SegmentDistributionItemDto
{
    public string SegmentName { get; set; } = null!;
    public string? ColorHex { get; set; }
    public int MemberCount { get; set; }
    public decimal Percentage { get; set; }
}
