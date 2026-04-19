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
    private readonly IDisputeService _disputeService;
    private readonly IRefundService _refundService;

    public OrderController(
        IOrderService orderService,
        ICancellationService cancellationService,
        IOrderTransferService transferService,
        IBulkImportService bulkImportService,
        IDuplicateDetectionService duplicateService,
        IOrderWorthService worthService,
        IAddressSwapService addressSwapService,
        IWaitingTimerService waitingTimerService,
        IDisputeService disputeService,
        IRefundService refundService)
    {
        _orderService = orderService;
        _cancellationService = cancellationService;
        _transferService = transferService;
        _bulkImportService = bulkImportService;
        _duplicateService = duplicateService;
        _worthService = worthService;
        _addressSwapService = addressSwapService;
        _waitingTimerService = waitingTimerService;
        _disputeService = disputeService;
        _refundService = refundService;
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
    public async Task<IActionResult> CreateDisclaimer(Guid id, [FromBody] CreateDisclaimerDto dto)
    {
        // Store disclaimer as order note
        var result = await _orderService.UpdateAsync(GetDriverId(), id, new UpdateOrderDto
        {
            Notes = $"[إخلاء مسؤولية] {dto.ItemsDescription} | الحالة: {dto.Condition} | {(dto.CustomerAcknowledged ? "العميل موافق" : "العميل لم يوافق")}{(dto.Notes != null ? " | " + dto.Notes : "")}"
        });
        if (!result.IsSuccess)
            return ToActionResult(result);
        return ToActionResult(Result<DisclaimerDto>.Success(new DisclaimerDto
        {
            Id = Guid.NewGuid(),
            OrderId = id,
            ItemsDescription = dto.ItemsDescription,
            Condition = dto.Condition,
            CustomerAcknowledged = dto.CustomerAcknowledged,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        }), StatusCodes.Status201Created);
    }

    [HttpGet("{id:guid}/disclaimer")]
    public async Task<IActionResult> GetDisclaimer(Guid id)
    {
        var order = await _orderService.GetByIdAsync(GetDriverId(), id);
        if (!order.IsSuccess)
            return ToActionResult(order);
        var notes = order.Value?.Notes;
        if (string.IsNullOrEmpty(notes) || !notes.Contains("[إخلاء مسؤولية]"))
            return NotFound(ApiResponse<object>.Fail("لا يوجد إخلاء مسؤولية لهذا الطلب"));
        return Ok(ApiResponse<object>.Success(new { orderNotes = notes }));
    }

    [HttpPost("{id:guid}/dispute")]
    public async Task<IActionResult> CreateDispute(Guid id, [FromBody] Core.DTOs.Financial.CreateDisputeDto? dto = null)
    {
        dto ??= new Core.DTOs.Financial.CreateDisputeDto { Description = "نزاع على الطلب" };
        dto.OrderId = id;
        return ToActionResult(await _disputeService.CreateAsync(GetDriverId(), dto));
    }

    [HttpGet("{id:guid}/disputes")]
    public async Task<IActionResult> GetDisputes(Guid id)
        => ToActionResult(await _disputeService.GetDisputesAsync(id));

    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> CreateRefund(Guid id, [FromBody] Core.DTOs.Financial.CreateRefundDto? dto = null)
    {
        dto ??= new Core.DTOs.Financial.CreateRefundDto();
        dto.OrderId = id;
        return ToActionResult(await _refundService.CreateAsync(GetDriverId(), dto));
    }

    [HttpGet("{id:guid}/refunds")]
    public async Task<IActionResult> GetRefunds(Guid id)
        => ToActionResult(await _refundService.GetRefundsAsync(id));

    [HttpGet("time-slots")]
    public async Task<IActionResult> GetTimeSlots([FromQuery] DateOnly? date, [FromQuery] Guid? regionId)
    {
        var d = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var timeSlotService = HttpContext.RequestServices.GetRequiredService<ITimeSlotService>();
        return ToActionResult(await timeSlotService.GetAvailableSlotsAsync(d, regionId));
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

        var errorResponse = result.Value is not null
            ? new ApiResponse<T> { IsSuccess = false, Data = result.Value, Message = result.Error!.Message }
            : ApiResponse<T>.Fail(result.Error!.Message);

        return result.Error.Code switch
        {
            "NOT_FOUND" => NotFound(errorResponse),
            "UNAUTHORIZED" => Unauthorized(errorResponse),
            "CONFLICT" => Conflict(errorResponse),
            "NOT_IMPLEMENTED" => StatusCode(501, errorResponse),
            _ => BadRequest(errorResponse)
        };
    }
}
