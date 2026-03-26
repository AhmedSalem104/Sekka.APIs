using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/wallets")]
[Authorize(Roles = "Admin")]
public class AdminWalletController : ControllerBase
{
    [HttpGet]
    public IActionResult GetWallets([FromQuery] AdminWalletFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة المحافظ")));

    [HttpGet("{driverId:guid}")]
    public IActionResult GetDriverWallet(Guid driverId)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل المحفظة")));

    [HttpGet("{driverId:guid}/transactions")]
    public IActionResult GetDriverTransactions(Guid driverId, [FromQuery] WalletTransactionFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("معاملات المحفظة")));

    [HttpPost("adjust")]
    public IActionResult AdjustBalance([FromBody] WalletAdjustmentDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تعديل الرصيد")));

    [HttpPost("{driverId:guid}/freeze")]
    public IActionResult FreezeWallet(Guid driverId, [FromBody] WalletFreezeDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تجميد المحفظة")));

    [HttpPost("{driverId:guid}/unfreeze")]
    public IActionResult UnfreezeWallet(Guid driverId)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إلغاء تجميد المحفظة")));

    [HttpGet("summary")]
    public IActionResult GetSummary()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص المحافظ")));

    [HttpGet("over-threshold")]
    public IActionResult GetOverThreshold()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("المحافظ فوق الحد")));

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminWalletFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصدير المحافظ")));

    [HttpPost("{driverId:guid}/reset")]
    public IActionResult ResetWallet(Guid driverId)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إعادة تعيين المحفظة")));
}
