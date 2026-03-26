using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/invoices")]
[Authorize(Roles = "Admin")]
public class AdminInvoiceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetInvoices([FromQuery] AdminInvoiceFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة الفواتير")));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل الفاتورة")));

    [HttpPost]
    public IActionResult Create([FromBody] CreateInvoiceDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إنشاء فاتورة")));

    [HttpPut("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحديث حالة الفاتورة")));

    [HttpGet("{id:guid}/pdf")]
    public IActionResult DownloadPdf(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحميل PDF الفاتورة")));

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص الفواتير")));

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminInvoiceFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصدير الفواتير")));
}
