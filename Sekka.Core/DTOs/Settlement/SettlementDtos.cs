using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Settlement;

public class CreateSettlementDto
{
    public Guid PartnerId { get; set; }
    public decimal Amount { get; set; }
    public SettlementType SettlementType { get; set; }
    public string? Notes { get; set; }
    public bool SendWhatsApp { get; set; } = true;
}

public class SettlementDto
{
    public Guid Id { get; set; }
    public string PartnerName { get; set; } = null!;
    public decimal Amount { get; set; }
    public SettlementType SettlementType { get; set; }
    public int OrderCount { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public bool WhatsAppSent { get; set; }
    public DateTime SettledAt { get; set; }
}

public class SettlementFilterDto : PaginationDto
{
    public Guid? PartnerId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class PartnerBalanceDto
{
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public int TotalOrders { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalSettled { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime? LastSettlementDate { get; set; }
}

public class DailySettlementSummaryDto
{
    public DateOnly Date { get; set; }
    public decimal TotalCashCollected { get; set; }
    public decimal TotalSettled { get; set; }
    public decimal RemainingCash { get; set; }
    public List<PartnerBalanceDto> PartnerSummaries { get; set; } = new();
}
