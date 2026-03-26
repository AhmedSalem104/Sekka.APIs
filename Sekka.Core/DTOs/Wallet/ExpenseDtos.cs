using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Wallet;

public class CreateExpenseDto
{
    public ExpenseType ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
}

public class ExpenseDto
{
    public Guid Id { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public string? Notes { get; set; }
}

public class ExpenseFilterDto : PaginationDto
{
    public ExpenseType? ExpenseType { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}
