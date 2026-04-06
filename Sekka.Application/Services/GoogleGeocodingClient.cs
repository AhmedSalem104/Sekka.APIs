using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

/// <summary>
/// Google Maps Geocoding — أدق حاجة للعناوين العربي والمصري.
/// Enabled = false by default. Set GoogleMaps:Enabled = true and provide ApiKey to activate.
/// Pricing: $5 per 1000 requests.
/// </summary>
public class GoogleGeocodingClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly bool _enabled;
    private readonly ILogger<GoogleGeocodingClient> _logger;

    public GoogleGeocodingClient(HttpClient httpClient, IConfiguration config, ILogger<GoogleGeocodingClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = config["GoogleMaps:ApiKey"] ?? "";
        _enabled = bool.TryParse(config["GoogleMaps:Enabled"], out var e) && e && !string.IsNullOrEmpty(_apiKey);

        _httpClient.BaseAddress = new Uri("https://maps.googleapis.com");
    }

    public bool IsEnabled => _enabled;

    /// <summary>
    /// Geocode: عنوان نصي → coordinates (Google Maps Geocoding API).
    /// </summary>
    public async Task<List<GeocodeResult>> GeocodeAsync(string address, double? focusLat = null, double? focusLon = null, string? country = "EG")
    {
        if (!_enabled) return new List<GeocodeResult>();

        try
        {
            var url = $"/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}&language=ar";
            if (!string.IsNullOrEmpty(country))
                url += $"&components=country:{country}";
            if (focusLat.HasValue && focusLon.HasValue)
                url += $"&bounds={focusLat.Value - 0.1},{focusLon.Value - 0.1}|{focusLat.Value + 0.1},{focusLon.Value + 0.1}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Google Geocoding returned {Status}", (int)response.StatusCode);
                return new List<GeocodeResult>();
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var doc = await JsonDocument.ParseAsync(stream);
            var status = doc.RootElement.GetProperty("status").GetString();

            if (status != "OK")
            {
                _logger.LogWarning("Google Geocoding status: {Status}", status);
                return new List<GeocodeResult>();
            }

            var results = new List<GeocodeResult>();
            foreach (var result in doc.RootElement.GetProperty("results").EnumerateArray())
            {
                var location = result.GetProperty("geometry").GetProperty("location");
                var lat = location.GetProperty("lat").GetDouble();
                var lng = location.GetProperty("lng").GetDouble();
                var formattedAddress = result.GetProperty("formatted_address").GetString() ?? "";

                // Extract address components
                string? street = null, district = null, city = null;
                foreach (var component in result.GetProperty("address_components").EnumerateArray())
                {
                    var types = component.GetProperty("types");
                    var name = component.GetProperty("long_name").GetString();
                    foreach (var type in types.EnumerateArray())
                    {
                        var t = type.GetString();
                        if (t == "route") street = name;
                        else if (t == "sublocality" || t == "neighborhood") district = name;
                        else if (t == "administrative_area_level_1") city = name;
                    }
                }

                results.Add(new GeocodeResult(
                    Latitude: lat,
                    Longitude: lng,
                    FormattedAddress: formattedAddress,
                    Street: street,
                    District: district,
                    City: city,
                    Confidence: 1.0
                ));
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google Geocoding failed for: {Address}", address);
            return new List<GeocodeResult>();
        }
    }

    /// <summary>
    /// Reverse Geocode: coordinates → عنوان نصي.
    /// </summary>
    public async Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude)
    {
        if (!_enabled) return null;

        try
        {
            var url = $"/maps/api/geocode/json?latlng={latitude},{longitude}&key={_apiKey}&language=ar";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();
            var doc = await JsonDocument.ParseAsync(stream);

            if (doc.RootElement.GetProperty("status").GetString() != "OK") return null;

            var results = doc.RootElement.GetProperty("results");
            if (results.GetArrayLength() == 0) return null;

            var first = results[0];
            var formattedAddress = first.GetProperty("formatted_address").GetString() ?? "";

            string? street = null, district = null, city = null;
            foreach (var component in first.GetProperty("address_components").EnumerateArray())
            {
                var types = component.GetProperty("types");
                var name = component.GetProperty("long_name").GetString();
                foreach (var type in types.EnumerateArray())
                {
                    var t = type.GetString();
                    if (t == "route") street = name;
                    else if (t == "sublocality" || t == "neighborhood") district = name;
                    else if (t == "administrative_area_level_1") city = name;
                }
            }

            return new ReverseGeocodeResult(formattedAddress, street, district, city, latitude, longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google Reverse Geocode failed for ({Lat},{Lon})", latitude, longitude);
            return null;
        }
    }
}
