using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class BulkImportDto
{
    public string RawText { get; set; } = null!;
    public Guid? PartnerId { get; set; }
    public PaymentMethod DefaultPaymentMethod { get; set; } = PaymentMethod.Cash;
}

public class BulkImportResultDto
{
    public int TotalParsed { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public int DuplicatesFound { get; set; }
    public List<OrderDto> Orders { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class CheckDuplicateDto
{
    public string CustomerPhone { get; set; } = null!;
    public string DeliveryAddress { get; set; } = null!;
    public decimal? Amount { get; set; }
}

public class DuplicateResultDto
{
    public bool IsDuplicate { get; set; }
    public decimal MatchScore { get; set; }
    public OrderListDto? MatchedOrder { get; set; }
    public DuplicateAction SuggestedAction { get; set; }
}
