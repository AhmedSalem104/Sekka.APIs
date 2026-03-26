using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ocr")]
[Authorize]
public class OcrController : ControllerBase
{
    [HttpPost("scan-invoice")]
    public IActionResult ScanInvoice(IFormFile file)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("مسح الفواتير")));

    [HttpPost("scan-to-order")]
    public IActionResult ScanToOrder(IFormFile file)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("مسح الفواتير")));

    [HttpPost("scan-batch")]
    public IActionResult ScanBatch(List<IFormFile> files)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("مسح الفواتير")));
}
