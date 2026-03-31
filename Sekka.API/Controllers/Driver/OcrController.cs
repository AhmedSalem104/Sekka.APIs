using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Persistence;
using Sekka.Persistence.Entities;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ocr")]
[Authorize]
public class OcrController : ControllerBase
{
    private readonly SekkaDbContext _db;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public OcrController(SekkaDbContext db)
    {
        _db = db;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Scan an invoice image and extract data (name, phone, address, amount)
    /// </summary>
    [HttpPost("scan-invoice")]
    public async Task<IActionResult> ScanInvoice(IFormFile file)
    {
        var validation = ValidateFile(file);
        if (validation != null) return validation;

        var savedPath = await SaveFile(file, "invoices");

        // Basic OCR simulation — in production, integrate Google Vision / Azure CV
        var extractedData = new
        {
            imageUrl = savedPath,
            fileName = file.FileName,
            fileSize = file.Length,
            extractedFields = new
            {
                customerName = (string?)null,
                customerPhone = (string?)null,
                address = (string?)null,
                amount = (decimal?)null,
                items = new List<object>()
            },
            confidence = 0.0,
            message = "تم رفع الصورة بنجاح. استخراج البيانات يتطلب ربط خدمة OCR خارجية (Google Vision / Azure). حالياً يمكنك إدخال البيانات يدوياً.",
            status = "uploaded"
        };

        return Ok(ApiResponse<object>.Success(extractedData));
    }

    /// <summary>
    /// Scan an invoice and auto-create an order from extracted data
    /// </summary>
    [HttpPost("scan-to-order")]
    public async Task<IActionResult> ScanToOrder(IFormFile file)
    {
        var validation = ValidateFile(file);
        if (validation != null) return validation;

        var driverId = GetDriverId();
        var savedPath = await SaveFile(file, "invoices");

        // Create order with image reference
        var order = new Order
        {
            DriverId = driverId,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            DeliveryAddress = "عنوان من الفاتورة — يرجى التعديل",
            Amount = 0,
            Status = Core.Enums.OrderStatus.Pending,
            SourceType = Core.Enums.OrderSourceType.OCR,
            Notes = $"[OCR] تم إنشاء الطلب من مسح فاتورة: {file.FileName}",
            AssignedAt = DateTime.UtcNow
        };

        _db.Orders.Add(order);

        // Save photo linked to order
        var photo = new OrderPhoto
        {
            OrderId = order.Id,
            PhotoUrl = savedPath,
            PhotoType = Core.Enums.PhotoType.Invoice,
            TakenAt = DateTime.UtcNow
        };
        _db.OrderPhotos.Add(photo);

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            orderId = order.Id,
            orderNumber = order.OrderNumber,
            imageUrl = savedPath,
            message = "تم إنشاء الطلب من الفاتورة. يرجى مراجعة البيانات وتعديلها.",
            status = "order_created"
        }));
    }

    /// <summary>
    /// Scan multiple invoices at once
    /// </summary>
    [HttpPost("scan-batch")]
    public async Task<IActionResult> ScanBatch(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("يرجى رفع ملف واحد على الأقل"));

        if (files.Count > 10)
            return BadRequest(ApiResponse<object>.Fail("الحد الأقصى 10 ملفات في المرة الواحدة"));

        var driverId = GetDriverId();
        var results = new List<object>();
        var successCount = 0;
        var failCount = 0;

        foreach (var file in files)
        {
            try
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!AllowedExtensions.Contains(ext) || file.Length > MaxFileSize)
                {
                    results.Add(new { fileName = file.FileName, status = "failed", error = "نوع أو حجم الملف غير مسموح" });
                    failCount++;
                    continue;
                }

                var savedPath = await SaveFile(file, "invoices");

                var order = new Order
                {
                    DriverId = driverId,
                    OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
                    DeliveryAddress = "عنوان من الفاتورة — يرجى التعديل",
                    Amount = 0,
                    Status = Core.Enums.OrderStatus.Pending,
                    SourceType = Core.Enums.OrderSourceType.OCR,
                    Notes = $"[OCR Batch] {file.FileName}",
                    AssignedAt = DateTime.UtcNow
                };
                _db.Orders.Add(order);

                var photo = new OrderPhoto
                {
                    OrderId = order.Id,
                    PhotoUrl = savedPath,
                    PhotoType = Core.Enums.PhotoType.Invoice,
                    TakenAt = DateTime.UtcNow
                };
                _db.OrderPhotos.Add(photo);

                results.Add(new { fileName = file.FileName, status = "success", orderId = order.Id, orderNumber = order.OrderNumber, imageUrl = savedPath });
                successCount++;
            }
            catch
            {
                results.Add(new { fileName = file.FileName, status = "failed", error = "خطأ في المعالجة" });
                failCount++;
            }
        }

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            totalFiles = files.Count,
            successCount,
            failCount,
            results,
            message = $"تم معالجة {successCount} ملف بنجاح من أصل {files.Count}"
        }));
    }

    private IActionResult? ValidateFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("يرجى رفع صورة"));

        if (file.Length > MaxFileSize)
            return BadRequest(ApiResponse<object>.Fail("حجم الملف يتجاوز 5 ميجابايت"));

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(ApiResponse<object>.Fail("نوع الملف غير مسموح. الأنواع المسموحة: jpg, png, webp"));

        return null;
    }

    private async Task<string> SaveFile(IFormFile file, string folder)
    {
        var ext = Path.GetExtension(file.FileName).ToLower();
        var uploadsBase = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "uploads");
        var dir = Path.Combine(uploadsBase, folder, GetDriverId().ToString());
        Directory.CreateDirectory(dir);

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var savedName = $"{Guid.NewGuid():N}_{timestamp}{ext}";
        var filePath = Path.Combine(dir, savedName);

        using (var fs = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(fs);

        return $"/uploads/{folder}/{GetDriverId()}/{savedName}";
    }
}
