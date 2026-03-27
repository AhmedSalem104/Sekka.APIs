using Sekka.Core.Common;
using Sekka.Core.DTOs.Intelligence;

namespace Sekka.Core.Interfaces.Services;

public interface IRecommendationService
{
    Task<Result<List<CustomerRecommendationDto>>> GetRecommendationsAsync(Guid driverId, Guid customerId);
    Task<Result<bool>> MarkReadAsync(Guid driverId, Guid recommendationId);
    Task<Result<bool>> DismissAsync(Guid driverId, Guid recommendationId);
    Task<Result<bool>> MarkActedUponAsync(Guid driverId, Guid recommendationId);
}
