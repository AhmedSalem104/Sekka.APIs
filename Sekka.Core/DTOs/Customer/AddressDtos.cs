using Sekka.Core.DTOs.Common;
using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Customer;

public class AddressDto
{
    public Guid Id { get; set; }
    public string AddressText { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public AddressType AddressType { get; set; }
    public int VisitCount { get; set; }
    public string? Landmarks { get; set; }
    public string? DeliveryNotes { get; set; }
}

public class SaveAddressDto
{
    public Guid? CustomerId { get; set; }
    public string AddressText { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public AddressType AddressType { get; set; }
    public string? Landmarks { get; set; }
    public string? DeliveryNotes { get; set; }
}

public class UpdateAddressDto
{
    public string? AddressText { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Landmarks { get; set; }
    public string? DeliveryNotes { get; set; }
}

public class AddressSearchDto : PaginationDto
{
    public string? SearchTerm { get; set; }
    public Guid? CustomerId { get; set; }
    public AddressType? AddressType { get; set; }
}
