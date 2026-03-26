using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Settlement;

namespace Sekka.Core.DTOs.Admin;

public class AdminSettlementFilterDto : PaginationDto
{
    public Guid? DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class AdminSettlementDto : SettlementDto
{
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
}

public class PendingSettlementDto
{
    public Guid Id { get; set; }
    public string DriverName { get; set; } = null!;
    public string PartnerName { get; set; } = null!;
    public decimal Amount { get; set; }
    public int OrderCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RejectSettlementDto
{
    public string Reason { get; set; } = null!;
}

public class DailySettlementReportDto
{
    public DateOnly Date { get; set; }
    public int TotalSettlements { get; set; }
    public decimal TotalAmount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int PendingCount { get; set; }
}
