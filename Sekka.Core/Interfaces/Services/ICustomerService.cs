using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface ICustomerService
{
    Task<Result<PagedResult<CustomerListDto>>> GetCustomersAsync(Guid driverId, CustomerFilterDto filter);
    Task<Result<CustomerDetailDto>> GetByIdAsync(Guid driverId, Guid customerId);
    Task<Result<CustomerDetailDto>> GetByPhoneAsync(Guid driverId, string phone);
    Task<Result<CustomerDto>> UpdateAsync(Guid driverId, Guid customerId, UpdateCustomerDto dto);
    Task<Result<RatingDto>> RateAsync(Guid driverId, Guid customerId, CreateRatingDto dto);
    Task<Result<bool>> BlockAsync(Guid driverId, Guid customerId, BlockCustomerDto dto);
    Task<Result<bool>> UnblockAsync(Guid driverId, Guid customerId);
    Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid driverId, Guid customerId, PaginationDto pagination);
    Task<Result<List<CustomerInterestDto>>> GetInterestsAsync(Guid driverId, Guid customerId);
    Task<Result<CustomerEngagementDto>> GetEngagementAsync(Guid driverId, Guid customerId);
}
