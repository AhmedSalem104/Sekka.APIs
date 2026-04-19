using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/favorite-drivers")]
[Authorize]
public class FavoriteDriverController : ControllerBase
{
    private readonly IFavoriteDriverService _favoriteDriverService;

    public FavoriteDriverController(IFavoriteDriverService favoriteDriverService)
    {
        _favoriteDriverService = favoriteDriverService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetFavorites()
        => ToActionResult(await _favoriteDriverService.GetFavoritesAsync(GetDriverId()));

    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDriverDto dto)
        => ToActionResult(await _favoriteDriverService.AddFavoriteAsync(GetDriverId(), dto), StatusCodes.Status201Created, "تم إضافة الزميل المفضل بنجاح");

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid id)
        => ToActionResult(await _favoriteDriverService.RemoveFavoriteAsync(GetDriverId(), id), message: "تم حذف الزميل المفضل بنجاح");

    [HttpPut("{id:guid}/refresh")]
    public async Task<IActionResult> Refresh(Guid id)
        => ToActionResult(await _favoriteDriverService.RefreshAsync(GetDriverId(), id));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
