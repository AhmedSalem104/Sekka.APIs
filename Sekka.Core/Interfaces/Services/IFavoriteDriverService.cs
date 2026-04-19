using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface IFavoriteDriverService
{
    Task<Result<List<FavoriteDriverDto>>> GetFavoritesAsync(Guid driverId);
    Task<Result<FavoriteDriverDto>> AddFavoriteAsync(Guid driverId, AddFavoriteDriverDto dto);
    Task<Result<bool>> RemoveFavoriteAsync(Guid driverId, Guid id);
    Task<Result<FavoriteDriverDto>> RefreshAsync(Guid driverId, Guid id);
}
