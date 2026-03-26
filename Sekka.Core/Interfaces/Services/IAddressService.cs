using Sekka.Core.Common;
using Sekka.Core.DTOs.Customer;

namespace Sekka.Core.Interfaces.Services;

public interface IAddressService
{
    Task<Result<List<AddressDto>>> SearchAsync(Guid driverId, AddressSearchDto search);
    Task<Result<AddressDto>> SaveAsync(Guid driverId, SaveAddressDto dto);
    Task<Result<AddressDto>> UpdateAsync(Guid driverId, Guid id, UpdateAddressDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<List<AddressDto>>> AutocompleteAsync(Guid driverId, string q, double? latitude, double? longitude);
    Task<Result<List<AddressDto>>> NearbyAsync(Guid driverId, double latitude, double longitude, double radiusKm);
}
