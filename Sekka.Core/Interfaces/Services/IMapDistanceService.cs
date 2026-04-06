namespace Sekka.Core.Interfaces.Services;

/// <summary>
/// Service for calculating real road distances and optimizing routes
/// using an external map provider (OpenRouteService).
/// </summary>
public interface IMapDistanceService
{
    /// <summary>
    /// Get the real road distance and duration between two points.
    /// </summary>
    Task<RouteDistanceResult?> GetRouteDistanceAsync(double fromLat, double fromLon, double toLat, double toLon);

    /// <summary>
    /// Get a distance matrix between multiple origins and destinations.
    /// Returns distances[i][j] = distance from origins[i] to destinations[j].
    /// </summary>
    Task<DistanceMatrixResult?> GetDistanceMatrixAsync(List<GeoPoint> locations);

    /// <summary>
    /// Given a start point and a list of waypoints, return the optimal visit order.
    /// Uses the Travelling Salesman Problem (TSP) solver.
    /// </summary>
    Task<OptimizedRouteResult?> OptimizeWaypointsAsync(GeoPoint start, List<IndexedWaypoint> waypoints);

    /// <summary>
    /// Convert an address text to coordinates (Geocoding).
    /// Uses driver's current location as bias for better results.
    /// </summary>
    Task<List<GeocodeResult>> GeocodeAsync(string address, double? focusLat = null, double? focusLon = null, string? country = "EG");

    /// <summary>
    /// Convert coordinates to an address (Reverse Geocoding).
    /// </summary>
    Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude);
}

public record GeoPoint(double Latitude, double Longitude);

public record IndexedWaypoint(Guid Id, double Latitude, double Longitude);

public record RouteDistanceResult(double DistanceKm, double DurationMinutes);

public record DistanceMatrixResult(double[][] DistancesKm, double[][] DurationsMinutes);

public record OptimizedRouteResult(
    List<Guid> OptimizedOrder,
    double TotalDistanceKm,
    double TotalDurationMinutes
);

public record GeocodeResult(
    double Latitude,
    double Longitude,
    string FormattedAddress,
    string? Street,
    string? District,
    string? City,
    double Confidence
);

public record ReverseGeocodeResult(
    string FormattedAddress,
    string? Street,
    string? District,
    string? City,
    double Latitude,
    double Longitude
);
