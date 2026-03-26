using Sekka.Core.Common;
using Sekka.Core.DTOs.Route;

namespace Sekka.Core.Interfaces.Services;

public interface IRouteService
{
    Task<Result<RouteDto>> OptimizeRouteAsync(Guid driverId, OptimizeRouteDto dto);
    Task<Result<RouteDto>> GetActiveRouteAsync(Guid driverId);
    Task<Result<RouteDto>> ReorderRouteAsync(Guid driverId, Guid routeId, ReorderRouteDto dto);
    Task<Result<RouteDto>> AddOrderToRouteAsync(Guid driverId, Guid routeId, AddOrderToRouteDto dto);
    Task<Result<RouteDto>> CompleteRouteAsync(Guid driverId, Guid routeId);
}
