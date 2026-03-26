using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICancellationService _cancellationService;
    private readonly IOrderTransferService _transferService;
    private readonly IBulkImportService _bulkImportService;
    private readonly IDuplicateDetectionService _duplicateService;
    private readonly IOrderWorthService _worthService;
    private readonly IAddressSwapService _addressSwapService;
    private readonly IWaitingTimerService _waitingTimerService;

    public OrderController(
        IOrderService orderService,
        ICancellationService cancellationService,
        IOrderTransferService transferService,
        IBulkImportService bulkImportService,
        IDuplicateDetectionService duplicateService,
        IOrderWorthService worthService,
        IAddressSwapService addressSwapService,
        IWaitingTimerService waitingTimerService)
    {
        _orderService = orderService;
        _cancellationService = cancellationService;
        _transferService = transferService;
        _bulkImportService = bulkImportService;
        _duplicateService = duplicateService;
        _worthService = worthService;
        _addressSwapService = addressSwapService;
        _waitingTimerService = waitingTimerService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        => ToActionResult(await _orderService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created, SuccessMessages.OrderCreated);

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrderFilterDto filter)
        => ToActionResult(await _orderService.GetOrdersAsync(GetDriverId(), filter));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => ToActionResult(await _orderService.GetByIdAsync(GetDriverId(), id));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderDto dto)
        => ToActionResult(await _orderService.UpdateAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderUpdated);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _orderService.DeleteAsync(GetDriverId(), id), message: SuccessMessages.OrderDeleted);

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        => ToActionResult(await _orderService.UpdateStatusAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderStatusUpdated);

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(Guid id, [FromBody] DeliverOrderDto dto)
        => ToActionResult(await _orderService.DeliverAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderDelivered);

    [HttpPost("{id:guid}/fail")]
    public async Task<IActionResult> Fail(Guid id, [FromBody] FailOrderDto dto)
        => ToActionResult(await _orderService.FailAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderFailed);

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderDto dto)
        => ToActionResult(await _cancellationService.CancelOrderAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderCancelled);

    [HttpPost("{id:guid}/transfer")]
    public async Task<IActionResult> Transfer(Guid id, [FromBody] TransferOrderDto dto)
        => ToActionResult(await _transferService.TransferAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderTransferred);

    [HttpPost("{id:guid}/partial")]
    public async Task<IActionResult> PartialDeliver(Guid id, [FromBody] PartialDeliveryDto dto)
        => ToActionResult(await _orderService.PartialDeliverAsync(GetDriverId(), id, dto), message: SuccessMessages.OrderPartialDelivery);

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkImport([FromBody] BulkImportDto dto)
        => ToActionResult(await _bulkImportService.ImportAsync(GetDriverId(), dto), message: SuccessMessages.BulkImportCompleted);

    [HttpPost("check-duplicate")]
    public async Task<IActionResult> CheckDuplicate([FromBody] CheckDuplicateDto dto)
        => ToActionResult(await _duplicateService.CheckDuplicateAsync(GetDriverId(), dto));

    [HttpPost("{id:guid}/worth")]
    public async Task<IActionResult> CalculateWorth(Guid id)
        => ToActionResult(await _worthService.CalculateWorthAsync(GetDriverId(), id));

    [HttpPost("{id:guid}/photos")]
    public async Task<IActionResult> UploadPhoto(Guid id, IFormFile file, [FromQuery] int photoType = 0)
    {
        var result = await _orderService.UploadPhotoAsync(GetDriverId(), id, file.OpenReadStream(), file.FileName, photoType);
        return ToActionResult(result, message: SuccessMessages.PhotoUploaded);
    }

    [HttpPost("{id:guid}/swap-address")]
    public async Task<IActionResult> SwapAddress(Guid id, [FromBody] SwapAddressDto dto)
        => ToActionResult(await _addressSwapService.SwapAddressAsync(GetDriverId(), id, dto), message: SuccessMessages.AddressSwapped);

    [HttpPost("{id:guid}/waiting/start")]
    public async Task<IActionResult> StartWaiting(Guid id)
        => ToActionResult(await _waitingTimerService.StartTimerAsync(GetDriverId(), id), message: SuccessMessages.WaitingTimerStarted);

    [HttpPost("{id:guid}/waiting/stop")]
    public async Task<IActionResult> StopWaiting(Guid id)
        => ToActionResult(await _waitingTimerService.StopTimerAsync(GetDriverId(), id), message: SuccessMessages.WaitingTimerStopped);

    [HttpPost("calculate-price")]
    public async Task<IActionResult> CalculatePrice([FromBody] PriceCalculationRequestDto dto)
        => ToActionResult(await _worthService.CalculatePriceAsync(dto));

    [HttpPost("{id:guid}/disclaimer")]
    public IActionResult CreateDisclaimer(Guid id, [FromBody] CreateDisclaimerDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إخلاء المسؤولية")));

    [HttpGet("{id:guid}/disclaimer")]
    public IActionResult GetDisclaimer(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إخلاء المسؤولية")));

    [HttpPost("{id:guid}/dispute")]
    public IActionResult CreateDispute(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("النزاعات")));

    [HttpGet("{id:guid}/disputes")]
    public IActionResult GetDisputes(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("النزاعات")));

    [HttpPost("{id:guid}/refund")]
    public IActionResult CreateRefund(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("الاسترداد")));

    [HttpGet("{id:guid}/refunds")]
    public IActionResult GetRefunds(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("الاسترداد")));

    [HttpGet("time-slots")]
    public async Task<IActionResult> GetTimeSlots([FromQuery] DateOnly date, [FromQuery] Guid? regionId)
    {
        var timeSlotService = HttpContext.RequestServices.GetRequiredService<ITimeSlotService>();
        return ToActionResult(await timeSlotService.GetAvailableSlotsAsync(date, regionId));
    }

    [HttpPost("{id:guid}/book-slot")]
    public async Task<IActionResult> BookSlot(Guid id, [FromBody] BookSlotDto dto)
    {
        var timeSlotService = HttpContext.RequestServices.GetRequiredService<ITimeSlotService>();
        return ToActionResult(await timeSlotService.BookSlotAsync(GetDriverId(), id, dto), message: SuccessMessages.TimeSlotBooked);
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
