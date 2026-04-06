namespace Sekka.Core.Interfaces.Services;

public interface INavigationLinkService
{
    /// <summary>
    /// Generate navigation links for a single destination.
    /// </summary>
    NavigationLinks GetNavigationLinks(double latitude, double longitude, string? label = null);

    /// <summary>
    /// Generate a multi-stop navigation link with waypoints in order.
    /// Google Maps supports start + up to 9 waypoints + destination.
    /// </summary>
    MultiStopNavigationLink GetMultiStopLink(
        double startLat, double startLon,
        List<NavigationStop> stops);
}

public record NavigationLinks(
    string GoogleMapsUrl,
    string WazeUrl
);

public record MultiStopNavigationLink(
    string GoogleMapsUrl,
    string WazeFirstStopUrl,
    List<string> WazePerStopUrls
);

public record NavigationStop(double Latitude, double Longitude, string? Label = null);
