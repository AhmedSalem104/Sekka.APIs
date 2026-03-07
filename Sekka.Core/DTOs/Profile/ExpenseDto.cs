using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Profile;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
}

public class CreateExpenseDto
{
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; }
}

public class ExpenseFilterDto : PaginationDto
{
    public string? Category { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
