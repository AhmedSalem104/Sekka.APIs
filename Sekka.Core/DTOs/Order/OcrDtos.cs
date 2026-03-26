using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class OcrScanResultDto
{
    public string? CustomerName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal? Amount { get; set; }
    public string? Description { get; set; }
    public double Confidence { get; set; }
    public string RawText { get; set; } = null!;
}

public class OcrToOrderDto
{
    public string CustomerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}
