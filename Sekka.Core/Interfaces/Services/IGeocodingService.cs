namespace Sekka.Core.Interfaces.Services;

/// <summary>
/// Geocoding service — converts addresses to coordinates and vice versa.
/// Uses Google Maps (if enabled) with ORS fallback.
/// </summary>
public interface IGeocodingService
{
    string ActiveProvider { get; }
    Task<List<GeocodeResult>> GeocodeAsync(string address, double? focusLat = null, double? focusLon = null, string? country = "EG");
    Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude);
}
