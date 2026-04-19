using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Pages;

public class TrackModel : PageModel
{
    private readonly ITrackingLinkService _trackingLinkService;

    public TrackModel(ITrackingLinkService trackingLinkService)
    {
        _trackingLinkService = trackingLinkService;
    }

    public TrackingPageDto? TrackingData { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string shareToken)
    {
        if (string.IsNullOrWhiteSpace(shareToken))
        {
            ErrorMessage = "رابط التتبع غير صالح";
            return Page();
        }

        var result = await _trackingLinkService.GetTrackingPageAsync(shareToken);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Error?.Message ?? "حدث خطأ غير متوقع";
            return Page();
        }

        TrackingData = result.Value;
        return Page();
    }
}
