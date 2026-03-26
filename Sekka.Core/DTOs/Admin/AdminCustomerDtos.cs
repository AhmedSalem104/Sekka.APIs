using Sekka.Core.DTOs.Common;

namespace Sekka.Core.DTOs.Admin;

public class AdminCustomerFilterDto : PaginationDto
{
    public string? SearchTerm { get; set; }
    public bool? IsBlocked { get; set; }
    public Guid? DriverId { get; set; }
}

public class AdminCustomerDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string? Name { get; set; }
    public string DriverName { get; set; } = null!;
    public decimal AverageRating { get; set; }
    public int TotalDeliveries { get; set; }
    public bool IsBlocked { get; set; }
}

public class AdminCustomerDetailDto : AdminCustomerDto
{
    public int FailedDeliveries { get; set; }
    public string? BlockReason { get; set; }
    public string? Notes { get; set; }
    public DateTime? LastDeliveryDate { get; set; }
}

public class AdminBlockCustomerDto
{
    public string? Reason { get; set; }
}

public class CustomerReportDto
{
    public int TotalCustomers { get; set; }
    public int BlockedCustomers { get; set; }
    public decimal AverageRating { get; set; }
    public int NewCustomersThisPeriod { get; set; }
}
