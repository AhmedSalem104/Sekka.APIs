using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/customers")]
[Authorize(Roles = "Admin")]
public class AdminCustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public AdminCustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetCustomers([FromQuery] PaginationDto pagination, [FromQuery] string? searchTerm)
    {
        // TODO: Admin-level customer listing across all drivers
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة العملاء")));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetCustomer(Guid id)
    {
        // TODO: Admin-level customer detail (cross-driver)
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل العميل")));
    }

    [HttpPost("{id:guid}/block")]
    public IActionResult BlockCustomer(Guid id, [FromBody] BlockCustomerDto dto)
    {
        // TODO: Admin-level global block
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("حظر العملاء")));
    }

    [HttpPost("{id:guid}/unblock")]
    public IActionResult UnblockCustomer(Guid id)
    {
        // TODO: Admin-level global unblock
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إلغاء حظر العملاء")));
    }

    [HttpGet("stats")]
    public IActionResult GetStats([FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
    {
        // TODO: Admin-level customer statistics
        return BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إحصائيات العملاء")));
    }
}
