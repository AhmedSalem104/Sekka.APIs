using System.Globalization;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class NavigationLinkService : INavigationLinkService
{
    /// <summary>
    /// Generate Google Maps + Waze navigation links for a single destination.
    /// </summary>
    public NavigationLinks GetNavigationLinks(double latitude, double longitude, string? label = null)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        // Google Maps: opens turn-by-turn navigation
        var googleUrl = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lon}&travelmode=driving";

        // Waze: opens navigation
        var wazeUrl = $"https://waze.com/ul?ll={lat},{lon}&navigate=yes";

        return new NavigationLinks(googleUrl, wazeUrl);
    }

    /// <summary>
    /// Generate multi-stop navigation links.
    /// Google Maps supports: origin + waypoints (up to 9) + destination.
    /// Waze doesn't support multi-stop, so we give per-stop URLs.
    /// </summary>
    public MultiStopNavigationLink GetMultiStopLink(
        double startLat, double startLon,
        List<NavigationStop> stops)
    {
        if (stops.Count == 0)
            return new MultiStopNavigationLink("", "", new List<string>());

        var startLatStr = startLat.ToString(CultureInfo.InvariantCulture);
        var startLonStr = startLon.ToString(CultureInfo.InvariantCulture);

        // Google Maps multi-stop URL
        // Format: origin=X,Y&waypoints=A,B|C,D|E,F&destination=G,H
        var lastStop = stops[^1];
        var destLat = lastStop.Latitude.ToString(CultureInfo.InvariantCulture);
        var destLon = lastStop.Longitude.ToString(CultureInfo.InvariantCulture);

        var googleUrl = $"https://www.google.com/maps/dir/?api=1&origin={startLatStr},{startLonStr}&destination={destLat},{destLon}&travelmode=driving";

        if (stops.Count > 1)
        {
            // Add intermediate stops as waypoints (max 9)
            var waypointStops = stops.Take(stops.Count - 1).Take(9);
            var waypointsParam = string.Join("|",
                waypointStops.Select(s =>
                    $"{s.Latitude.ToString(CultureInfo.InvariantCulture)},{s.Longitude.ToString(CultureInfo.InvariantCulture)}"));
            googleUrl += $"&waypoints={Uri.EscapeDataString(waypointsParam)}";
        }

        // Waze: first stop only (Waze doesn't support multi-stop)
        var firstStop = stops[0];
        var wazeFirstUrl = $"https://waze.com/ul?ll={firstStop.Latitude.ToString(CultureInfo.InvariantCulture)},{firstStop.Longitude.ToString(CultureInfo.InvariantCulture)}&navigate=yes";

        // Per-stop Waze URLs so mobile can chain them
        var wazePerStop = stops.Select(s =>
            $"https://waze.com/ul?ll={s.Latitude.ToString(CultureInfo.InvariantCulture)},{s.Longitude.ToString(CultureInfo.InvariantCulture)}&navigate=yes"
        ).ToList();

        return new MultiStopNavigationLink(googleUrl, wazeFirstUrl, wazePerStop);
    }
}
