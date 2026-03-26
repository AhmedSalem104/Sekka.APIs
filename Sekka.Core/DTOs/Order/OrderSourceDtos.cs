using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class OrderSourceStatsDto
{
    public SourceTagType SourceType { get; set; }
    public string SourceName { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Percentage { get; set; }
}
