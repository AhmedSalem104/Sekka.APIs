using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Expense : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
