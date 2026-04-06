using Microsoft.Extensions.Logging;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

/// <summary>
/// Smart geocoding: Google Maps (paid, accurate) → ORS (free, fallback).
/// لو Google مفعّل ومعاه key → يستخدمه.
/// لو لا → يروح على ORS المجاني.
/// </summary>
public class CompositeGeocodingService : IGeocodingService
{
    private readonly GoogleGeocodingClient _google;
    private readonly IMapDistanceService _ors;
    private readonly ILogger<CompositeGeocodingService> _logger;

    public CompositeGeocodingService(
        GoogleGeocodingClient google,
        IMapDistanceService ors,
        ILogger<CompositeGeocodingService> logger)
    {
        _google = google;
        _ors = ors;
        _logger = logger;
    }

    public string ActiveProvider => _google.IsEnabled ? "Google Maps" : "OpenRouteService";

    public async Task<List<GeocodeResult>> GeocodeAsync(string address, double? focusLat = null, double? focusLon = null, string? country = "EG")
    {
        // Try Google first if enabled
        if (_google.IsEnabled)
        {
            var googleResults = await _google.GeocodeAsync(address, focusLat, focusLon, country);
            if (googleResults.Count > 0)
            {
                _logger.LogInformation("Geocode via Google Maps: {Count} results for '{Address}'", googleResults.Count, address);
                return googleResults;
            }
        }

        // Fallback to ORS (free)
        var orsResults = await _ors.GeocodeAsync(address, focusLat, focusLon, country);
        _logger.LogInformation("Geocode via ORS: {Count} results for '{Address}'", orsResults.Count, address);
        return orsResults;
    }

    public async Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude)
    {
        // Try Google first if enabled
        if (_google.IsEnabled)
        {
            var googleResult = await _google.ReverseGeocodeAsync(latitude, longitude);
            if (googleResult is not null) return googleResult;
        }

        // Fallback to ORS
        return await _ors.ReverseGeocodeAsync(latitude, longitude);
    }
}
