using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/map")]
[Authorize]
public class MapController : ControllerBase
{
    private readonly IMapDistanceService _mapService;
    private readonly IGeocodingService _geocodingService;
    private readonly INavigationLinkService _navService;

    public MapController(IMapDistanceService mapService, IGeocodingService geocodingService, INavigationLinkService navService)
    {
        _mapService = mapService;
        _geocodingService = geocodingService;
        _navService = navService;
    }

    // ═══════════════════════════════════════════════════════════
    // Geocoding
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// تحويل عنوان نصي إلى إحداثيات (Geocode).
    /// أرسل lat/lng بتاع السواق الحالي عشان النتائج تكون دقيقة.
    /// Provider: Google Maps (لو مفعّل) أو OpenRouteService (مجاني).
    /// </summary>
    [HttpGet("geocode")]
    public async Task<IActionResult> Geocode(
        [FromQuery] string address,
        [FromQuery] double? lat = null,
        [FromQuery] double? lng = null,
        [FromQuery] string? country = "EG")
    {
        if (string.IsNullOrWhiteSpace(address))
            return BadRequest(ApiResponse<object>.Fail("العنوان مطلوب"));

        var results = await _geocodingService.GeocodeAsync(address, lat, lng, country);
        return Ok(ApiResponse<List<GeocodeResult>>.Success(results, $"Provider: {_geocodingService.ActiveProvider}"));
    }

    /// <summary>
    /// تحويل إحداثيات إلى عنوان نصي (Reverse Geocode).
    /// يُستخدم لما السواق يحط pin على الخريطة.
    /// </summary>
    [HttpGet("reverse-geocode")]
    public async Task<IActionResult> ReverseGeocode([FromQuery] double lat, [FromQuery] double lng)
    {
        var result = await _geocodingService.ReverseGeocodeAsync(lat, lng);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("لم يتم العثور على عنوان لهذه الإحداثيات"));

        return Ok(ApiResponse<ReverseGeocodeResult>.Success(result));
    }

    // ═══════════════════════════════════════════════════════════
    // Road Distance
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// حساب المسافة الحقيقية على الطريق بين نقطتين.
    /// </summary>
    [HttpGet("distance")]
    public async Task<IActionResult> GetDistance(
        [FromQuery] double fromLat, [FromQuery] double fromLng,
        [FromQuery] double toLat, [FromQuery] double toLng)
    {
        var result = await _mapService.GetRouteDistanceAsync(fromLat, fromLng, toLat, toLng);
        if (result is null)
            return BadRequest(ApiResponse<object>.Fail("تعذر حساب المسافة"));

        return Ok(ApiResponse<RouteDistanceResult>.Success(result));
    }

    // ═══════════════════════════════════════════════════════════
    // Navigation Links
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// توليد لينكات تنقل (Google Maps + Waze) لوجهة واحدة.
    /// </summary>
    [HttpGet("navigate")]
    public IActionResult GetNavigationLinks([FromQuery] double lat, [FromQuery] double lng, [FromQuery] string? label = null)
    {
        var links = _navService.GetNavigationLinks(lat, lng, label);
        return Ok(ApiResponse<NavigationLinks>.Success(links));
    }

    /// <summary>
    /// توليد لينك تنقل متعدد المحطات (Multi-stop) للمسار المُحسَّن.
    /// </summary>
    [HttpPost("navigate/multi-stop")]
    public IActionResult GetMultiStopNavigation([FromBody] MultiStopRequest request)
    {
        if (request.Stops == null || request.Stops.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("يجب إضافة محطة واحدة على الأقل"));

        var stops = request.Stops
            .Select(s => new NavigationStop(s.Latitude, s.Longitude, s.Label))
            .ToList();

        var links = _navService.GetMultiStopLink(request.StartLatitude, request.StartLongitude, stops);
        return Ok(ApiResponse<MultiStopNavigationLink>.Success(links));
    }
}

// ═══════════════════════════════════════════════════════════
// Request DTOs
// ═══════════════════════════════════════════════════════════

public class MultiStopRequest
{
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public List<StopDto> Stops { get; set; } = new();
}

public class StopDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Label { get; set; }
}
