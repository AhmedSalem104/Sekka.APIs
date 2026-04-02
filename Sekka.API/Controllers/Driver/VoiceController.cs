using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

/// <summary>Voice input endpoints for guided voice order creation</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/voice")]
[Authorize]
public class VoiceController : ControllerBase
{
    private readonly ISpeechToTextService _speechService;
    private readonly ILogger<VoiceController> _logger;

    public VoiceController(ISpeechToTextService speechService, ILogger<VoiceController> logger)
    {
        _speechService = speechService;
        _logger = logger;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Transcribe audio to text for guided voice input (send one field at a time)</summary>
    /// <remarks>
    /// Field hints: customerName, phone, address, amount, notes, itemCount, paymentMethod
    /// Supported formats: wav, mp3, m4a, ogg, webm
    /// Max file size: 5 MB
    /// </remarks>
    [HttpPost("transcribe")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Transcribe(
        IFormFile audio,
        [FromForm] string fieldHint = "general")
    {
        if (audio == null || audio.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("لم يتم إرسال ملف صوتي"));

        if (audio.Length > 5 * 1024 * 1024)
            return BadRequest(ApiResponse<object>.Fail("حجم الملف أكبر من 5 ميجا"));

        var allowedExtensions = new[] { ".wav", ".mp3", ".m4a", ".ogg", ".webm", ".opus" };
        var ext = Path.GetExtension(audio.FileName)?.ToLowerInvariant();
        if (ext == null || !allowedExtensions.Contains(ext))
            return BadRequest(ApiResponse<object>.Fail("صيغة الملف غير مدعومة — استخدم wav, mp3, m4a, ogg, webm"));

        _logger.LogInformation("Voice transcription request from driver {DriverId}, field: {Field}, size: {Size}bytes",
            GetDriverId(), fieldHint, audio.Length);

        using var stream = audio.OpenReadStream();
        var result = await _speechService.TranscribeAsync(stream, audio.FileName, fieldHint);

        return ToActionResult(result);
    }

    /// <summary>Get supported field hints and their descriptions</summary>
    [HttpGet("fields")]
    public IActionResult GetFieldHints()
    {
        var fields = new[]
        {
            new { field = "customerName", description = "اسم العميل", example = "أحمد محمد", smartParse = false },
            new { field = "phone", description = "رقم التليفون", example = "01012345678", smartParse = true },
            new { field = "address", description = "عنوان التوصيل", example = "شارع التحرير، الدقي", smartParse = false },
            new { field = "amount", description = "المبلغ", example = "150", smartParse = true },
            new { field = "notes", description = "ملاحظات", example = "الباب الجانبي", smartParse = false },
            new { field = "itemCount", description = "عدد القطع", example = "3", smartParse = true },
            new { field = "paymentMethod", description = "طريقة الدفع", example = "كاش", smartParse = false },
            new { field = "description", description = "وصف الطلب", example = "صندوق ملابس", smartParse = false },
            new { field = "general", description = "نص عام", example = "أي كلام", smartParse = false }
        };

        return Ok(ApiResponse<object>.Success(new
        {
            fields,
            supportedFormats = new[] { "wav", "mp3", "m4a", "ogg", "webm" },
            maxFileSizeMB = 5,
            language = "ar-EG (عربي مصري)"
        }));
    }

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
