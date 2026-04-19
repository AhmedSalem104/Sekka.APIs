using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Customer;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string? Name { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalDeliveries { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? LastDeliveryDate { get; set; }
}

public class CustomerListDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string? Name { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalDeliveries { get; set; }
    public bool IsBlocked { get; set; }
}

public class CustomerDetailDto : CustomerDto
{
    public int FailedDeliveries { get; set; }
    public string? BlockReason { get; set; }
    public string? Notes { get; set; }
    public PaymentMethod? PreferredPaymentMethod { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();
    public List<Order.OrderListDto> RecentOrders { get; set; } = new();
    public List<RatingDto> Ratings { get; set; } = new();
}

public class CustomerFilterDto : PaginationDto
{
    public string? SearchTerm { get; set; }
    public bool? IsBlocked { get; set; }
    public decimal? MinRating { get; set; }
    public string SortBy { get; set; } = "lastDelivery";
}

public class UpdateCustomerDto
{
    public string? Name { get; set; }
    public string? Notes { get; set; }
    public PaymentMethod? PreferredPaymentMethod { get; set; }
}

public class CustomerInterestDto
{
    public string Category { get; set; } = null!;
    public decimal Score { get; set; }
    public int OrderCount { get; set; }
}

public class CustomerEngagementDto
{
    public int TotalOrders { get; set; }
    public decimal EngagementScore { get; set; }
    public string Level { get; set; } = null!;
    public DateTime? LastInteraction { get; set; }
    public int? DaysSinceLastOrder { get; set; }
}
