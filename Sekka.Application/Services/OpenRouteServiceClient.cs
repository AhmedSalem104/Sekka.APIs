using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class OpenRouteServiceClient : IMapDistanceService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<OpenRouteServiceClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public OpenRouteServiceClient(HttpClient httpClient, IConfiguration config, ILogger<OpenRouteServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _apiKey = config["OpenRouteService:ApiKey"] ?? "";

        var baseUrl = config["OpenRouteService:BaseUrl"] ?? "https://api.openrouteservice.org";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", _apiKey);
    }

    // ═══════════════════════════════════════════════════════════
    // 1. Route Distance — المسافة الحقيقية بين نقطتين
    // ═══════════════════════════════════════════════════════════
    public async Task<RouteDistanceResult?> GetRouteDistanceAsync(
        double fromLat, double fromLon, double toLat, double toLon)
    {
        try
        {
            // ORS expects [longitude, latitude] format
            var body = new
            {
                coordinates = new[] { new[] { fromLon, fromLat }, new[] { toLon, toLat } }
            };

            var response = await PostJsonAsync("/v2/directions/driving-car", body);
            if (response is null) return null;

            var routes = response.RootElement.GetProperty("routes");
            if (routes.GetArrayLength() == 0) return null;

            var summary = routes[0].GetProperty("summary");
            var distanceMeters = summary.GetProperty("distance").GetDouble();
            var durationSeconds = summary.GetProperty("duration").GetDouble();

            return new RouteDistanceResult(
                DistanceKm: Math.Round(distanceMeters / 1000.0, 2),
                DurationMinutes: Math.Round(durationSeconds / 60.0, 1)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ORS Directions API failed for ({FromLat},{FromLon}) -> ({ToLat},{ToLon})",
                fromLat, fromLon, toLat, toLon);
            return null;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 2. Distance Matrix — مصفوفة المسافات بين كل النقط
    // ═══════════════════════════════════════════════════════════
    public async Task<DistanceMatrixResult?> GetDistanceMatrixAsync(List<GeoPoint> locations)
    {
        if (locations.Count < 2) return null;

        try
        {
            var body = new
            {
                locations = locations.Select(p => new[] { p.Longitude, p.Latitude }).ToArray(),
                metrics = new[] { "distance", "duration" }
            };

            var response = await PostJsonAsync("/v2/matrix/driving-car", body);
            if (response is null) return null;

            var distances = ParseMatrix(response.RootElement.GetProperty("distances"));
            var durations = ParseMatrix(response.RootElement.GetProperty("durations"));

            // Convert meters to km and seconds to minutes
            for (int i = 0; i < distances.Length; i++)
                for (int j = 0; j < distances[i].Length; j++)
                {
                    distances[i][j] = Math.Round(distances[i][j] / 1000.0, 2);
                    durations[i][j] = Math.Round(durations[i][j] / 60.0, 1);
                }

            return new DistanceMatrixResult(distances, durations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ORS Matrix API failed for {Count} locations", locations.Count);
            return null;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 3. Optimize Waypoints — ترتيب الأوردرات بأحسن ترتيب
    // ═══════════════════════════════════════════════════════════
    public async Task<OptimizedRouteResult?> OptimizeWaypointsAsync(
        GeoPoint start, List<IndexedWaypoint> waypoints)
    {
        if (waypoints.Count == 0) return null;

        // For single waypoint, no optimization needed
        if (waypoints.Count == 1)
        {
            var dist = await GetRouteDistanceAsync(
                start.Latitude, start.Longitude,
                waypoints[0].Latitude, waypoints[0].Longitude);

            return new OptimizedRouteResult(
                OptimizedOrder: new List<Guid> { waypoints[0].Id },
                TotalDistanceKm: dist?.DistanceKm ?? 0,
                TotalDurationMinutes: dist?.DurationMinutes ?? 0
            );
        }

        try
        {
            // Use ORS Optimization API (VROOM-based TSP solver)
            var jobs = waypoints.Select((wp, idx) => new
            {
                id = idx + 1, // VROOM jobs are 1-indexed
                location = new[] { wp.Longitude, wp.Latitude }
            }).ToArray();

            var body = new
            {
                jobs,
                vehicles = new[]
                {
                    new
                    {
                        id = 1,
                        profile = "driving-car",
                        start = new[] { start.Longitude, start.Latitude }
                    }
                }
            };

            var response = await PostJsonAsync("/optimization", body);
            if (response is null) return FallbackNearestNeighbor(start, waypoints);

            var routes = response.RootElement.GetProperty("routes");
            if (routes.GetArrayLength() == 0) return FallbackNearestNeighbor(start, waypoints);

            var route = routes[0];
            var steps = route.GetProperty("steps");

            // Extract optimized order and coordinates from steps (skip "start" step)
            var optimizedOrder = new List<Guid>();
            var routeCoords = new List<double[]>
            {
                new[] { start.Longitude, start.Latitude } // start point
            };

            foreach (var step in steps.EnumerateArray())
            {
                if (step.GetProperty("type").GetString() == "job")
                {
                    var jobId = step.GetProperty("job").GetInt32();
                    var waypointIndex = jobId - 1; // Convert back to 0-indexed
                    if (waypointIndex >= 0 && waypointIndex < waypoints.Count)
                    {
                        optimizedOrder.Add(waypoints[waypointIndex].Id);
                        routeCoords.Add(new[] { waypoints[waypointIndex].Longitude, waypoints[waypointIndex].Latitude });
                    }
                }
            }

            var durationSeconds = route.GetProperty("duration").GetDouble();

            // Optimization API doesn't return distance — call Directions API for actual road distance
            double totalDistanceKm = 0;
            var dirBody = new { coordinates = routeCoords.ToArray() };
            var dirResponse = await PostJsonAsync("/v2/directions/driving-car", dirBody);
            if (dirResponse is not null)
            {
                var dirRoutes = dirResponse.RootElement.GetProperty("routes");
                if (dirRoutes.GetArrayLength() > 0)
                {
                    var summary = dirRoutes[0].GetProperty("summary");
                    totalDistanceKm = Math.Round(summary.GetProperty("distance").GetDouble() / 1000.0, 2);
                }
            }

            return new OptimizedRouteResult(
                OptimizedOrder: optimizedOrder,
                TotalDistanceKm: totalDistanceKm,
                TotalDurationMinutes: Math.Round(durationSeconds / 60.0, 1)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ORS Optimization API failed for {Count} waypoints", waypoints.Count);
            return FallbackNearestNeighbor(start, waypoints);
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 4. Geocoding — تحويل العنوان لـ coordinates
    // ═══════════════════════════════════════════════════════════
    public async Task<List<GeocodeResult>> GeocodeAsync(string address, double? focusLat = null, double? focusLon = null, string? country = "EG")
    {
        try
        {
            var query = Uri.EscapeDataString(address);
            var url = $"/geocode/search?api_key={_apiKey}&text={query}&size=5";
            if (!string.IsNullOrEmpty(country))
                url += $"&boundary.country={country}";
            // Bias results toward driver's current location
            if (focusLat.HasValue && focusLon.HasValue)
                url += $"&focus.point.lat={focusLat.Value}&focus.point.lon={focusLon.Value}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("ORS Geocode API returned {Status}: {Error}", (int)response.StatusCode, error);
                return new List<GeocodeResult>();
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var doc = await JsonDocument.ParseAsync(stream);
            var features = doc.RootElement.GetProperty("features");

            var results = new List<GeocodeResult>();
            foreach (var feature in features.EnumerateArray())
            {
                var coords = feature.GetProperty("geometry").GetProperty("coordinates");
                var props = feature.GetProperty("properties");

                results.Add(new GeocodeResult(
                    Latitude: coords[1].GetDouble(),
                    Longitude: coords[0].GetDouble(),
                    FormattedAddress: props.TryGetProperty("label", out var label) ? label.GetString() ?? "" : "",
                    Street: props.TryGetProperty("street", out var street) ? street.GetString() : null,
                    District: props.TryGetProperty("locality", out var locality) ? locality.GetString() : null,
                    City: props.TryGetProperty("region", out var region) ? region.GetString() : null,
                    Confidence: props.TryGetProperty("confidence", out var conf) ? conf.GetDouble() : 0
                ));
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ORS Geocode failed for address: {Address}", address);
            return new List<GeocodeResult>();
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 5. Reverse Geocoding — تحويل coordinates لـ عنوان
    // ═══════════════════════════════════════════════════════════
    public async Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude)
    {
        try
        {
            var url = $"/geocode/reverse?api_key={_apiKey}&point.lat={latitude}&point.lon={longitude}&size=1";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("ORS Reverse Geocode returned {Status}: {Error}", (int)response.StatusCode, error);
                return null;
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var doc = await JsonDocument.ParseAsync(stream);
            var features = doc.RootElement.GetProperty("features");

            if (features.GetArrayLength() == 0) return null;

            var props = features[0].GetProperty("properties");

            return new ReverseGeocodeResult(
                FormattedAddress: props.TryGetProperty("label", out var label) ? label.GetString() ?? "" : "",
                Street: props.TryGetProperty("street", out var street) ? street.GetString() : null,
                District: props.TryGetProperty("locality", out var locality) ? locality.GetString() : null,
                City: props.TryGetProperty("region", out var region) ? region.GetString() : null,
                Latitude: latitude,
                Longitude: longitude
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ORS Reverse Geocode failed for ({Lat},{Lon})", latitude, longitude);
            return null;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // Helpers
    // ═══════════════════════════════════════════════════════════

    private async Task<JsonDocument?> PostJsonAsync(string endpoint, object body)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("ORS API {Endpoint} returned {Status}: {Error}",
                endpoint, (int)response.StatusCode, errorBody);
            return null;
        }

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }

    private static double[][] ParseMatrix(JsonElement matrixElement)
    {
        var rows = matrixElement.GetArrayLength();
        var matrix = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            var row = matrixElement[i];
            var cols = row.GetArrayLength();
            matrix[i] = new double[cols];
            for (int j = 0; j < cols; j++)
                matrix[i][j] = row[j].GetDouble();
        }
        return matrix;
    }

    /// <summary>
    /// Fallback: Nearest-neighbor greedy algorithm when ORS Optimization API fails.
    /// </summary>
    private OptimizedRouteResult FallbackNearestNeighbor(GeoPoint start, List<IndexedWaypoint> waypoints)
    {
        _logger.LogWarning("Using fallback nearest-neighbor for {Count} waypoints", waypoints.Count);

        var remaining = new List<IndexedWaypoint>(waypoints);
        var ordered = new List<Guid>();
        var currentLat = start.Latitude;
        var currentLon = start.Longitude;
        var totalDistance = 0.0;

        while (remaining.Count > 0)
        {
            var nearest = remaining
                .Select(wp => new { Waypoint = wp, Dist = HaversineKm(currentLat, currentLon, wp.Latitude, wp.Longitude) })
                .OrderBy(x => x.Dist)
                .First();

            ordered.Add(nearest.Waypoint.Id);
            totalDistance += nearest.Dist;
            currentLat = nearest.Waypoint.Latitude;
            currentLon = nearest.Waypoint.Longitude;
            remaining.Remove(nearest.Waypoint);
        }

        return new OptimizedRouteResult(ordered, Math.Round(totalDistance, 2), 0);
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
