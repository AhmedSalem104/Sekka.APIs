using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Admin;

public class AdminPartnerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public PartnerType PartnerType { get; set; }
    public string DriverName { get; set; } = null!;
    public bool IsActive { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public int OrdersCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminCreatePartnerDto
{
    public Guid DriverId { get; set; }
    public string Name { get; set; } = null!;
    public PartnerType PartnerType { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class AdminUpdatePartnerDto
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
}

public class PartnerAnalyticsDto
{
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class AdminVerifyPartnerDto
{
    public VerificationStatus Status { get; set; }
    public string? Note { get; set; }
}

public class RequestDocumentDto
{
    public string DocumentType { get; set; } = null!;
    public string? Message { get; set; }
}
