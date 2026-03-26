using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Partner;

public class PartnerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public PartnerType PartnerType { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public CommissionType CommissionType { get; set; }
    public decimal CommissionValue { get; set; }
    public string Color { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
}

public class CreatePartnerDto
{
    public string Name { get; set; } = null!;
    public PartnerType PartnerType { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public CommissionType CommissionType { get; set; }
    public decimal CommissionValue { get; set; }
    public PaymentMethod DefaultPaymentMethod { get; set; }
    public string? Color { get; set; }
    public string? ReceiptHeader { get; set; }
}

public class UpdatePartnerDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public CommissionType? CommissionType { get; set; }
    public decimal? CommissionValue { get; set; }
    public string? Color { get; set; }
    public string? LogoUrl { get; set; }
    public string? ReceiptHeader { get; set; }
    public bool? IsActive { get; set; }
}

public class PartnerVerificationDto
{
    public VerificationStatus Status { get; set; }
    public string? DocumentUrl { get; set; }
    public string? Note { get; set; }
    public DateTime? VerifiedAt { get; set; }
}
