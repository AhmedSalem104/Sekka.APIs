using System.ComponentModel.DataAnnotations;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Financial;

public class CreateExpenseDto
{
    public ExpenseType ExpenseType { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "المبلغ لازم يكون أكبر من صفر")]
    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
}
