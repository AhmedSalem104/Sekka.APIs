using System.ComponentModel.DataAnnotations;
using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class SettlementDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public Guid PartnerId { get; set; }
    public string? PartnerName { get; set; }
    public decimal Amount { get; set; }
    public SettlementType SettlementType { get; set; }
    public int OrderCount { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public bool WhatsAppSent { get; set; }
    public DateTime SettledAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSettlementDto
{
    [Required]
    public Guid PartnerId { get; set; }
    public decimal Amount { get; set; }
    public SettlementType SettlementType { get; set; }
    public int OrderCount { get; set; }
    public string? Notes { get; set; }
}

public class SettlementFilterDto : PaginationDto
{
    public Guid? PartnerId { get; set; }
    public SettlementType? SettlementType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class PartnerBalanceDto
{
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public decimal TotalCollected { get; set; }
    public decimal TotalSettled { get; set; }
    public decimal PendingBalance { get; set; }
    public int PendingOrderCount { get; set; }
}

public class DailySettlementSummaryDto
{
    public DateOnly Date { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalSettled { get; set; }
    public decimal RemainingBalance { get; set; }
    public int SettlementCount { get; set; }
    public int PendingPartners { get; set; }
}

public class AdminSettlementFilterDto : PaginationDto
{
    public string? Search { get; set; }
    public Guid? DriverId { get; set; }
    public SettlementType? SettlementType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
