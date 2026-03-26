using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class CreateExpenseDto
{
    public ExpenseType ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
}
