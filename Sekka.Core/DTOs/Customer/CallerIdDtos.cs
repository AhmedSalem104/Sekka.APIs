using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Customer;

public class CallerIdDto
{
    public string PhoneNumber { get; set; } = null!;
    public string? DisplayName { get; set; }
    public ContactType ContactType { get; set; }
    public string? CustomerName { get; set; }
    public string? PartnerName { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public decimal? AverageRating { get; set; }
    public string? Note { get; set; }
    public bool IsBlocked { get; set; }
}

public class CreateCallerIdNoteDto
{
    public string PhoneNumber { get; set; } = null!;
    public ContactType ContactType { get; set; }
    public string? DisplayName { get; set; }
    public string? Note { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? PartnerId { get; set; }
}

public class UpdateCallerIdNoteDto
{
    public ContactType? ContactType { get; set; }
    public string? DisplayName { get; set; }
    public string? Note { get; set; }
}

public class CallerIdNoteDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public ContactType ContactType { get; set; }
    public string? DisplayName { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TruecallerLookupDto
{
    public string Phone { get; set; } = null!;
    public string? Name { get; set; }
    public int SpamScore { get; set; }
    public bool IsVerified { get; set; }
    public bool InternalCustomer { get; set; }
    public double? InternalRating { get; set; }
    public int InternalOrdersCount { get; set; }
    public string RiskLevel { get; set; } = "Low";
}
