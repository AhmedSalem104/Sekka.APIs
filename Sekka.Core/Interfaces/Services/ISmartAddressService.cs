using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface ISmartAddressService
{
    Task<Result<SmartAddressDto>> GetSmartAddressAsync(string rawAddress);
    Task<Result<List<SmartAddressDto>>> SearchAddressAsync(string query, double? nearLatitude, double? nearLongitude);
    Task<Result<SmartAddressDto>> GeocodeAsync(double latitude, double longitude);
}
