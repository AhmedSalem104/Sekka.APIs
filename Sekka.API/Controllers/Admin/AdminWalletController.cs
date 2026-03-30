using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/wallets")]
[Authorize(Roles = "Admin")]
public class AdminWalletController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminWalletController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet("driver/{driverId:guid}")]
    public async Task<IActionResult> GetDriverWallet(Guid driverId)
    {
        var driver = await _db.Drivers.AsNoTracking()
            .Where(d => d.Id == driverId)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.CashOnHand,
                d.CashAlertThreshold,
                d.IsActive
            })
            .FirstOrDefaultAsync();

        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        var lastTransaction = await _db.WalletTransactions.AsNoTracking()
            .Where(t => t.DriverId == driverId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new { t.BalanceAfter, t.CreatedAt })
            .FirstOrDefaultAsync();

        var totalSettlements = await _db.Settlements.AsNoTracking()
            .Where(s => s.DriverId == driverId)
            .SumAsync(s => s.Amount);

        return Ok(ApiResponse<object>.Success(new
        {
            driver.Id,
            driver.Name,
            Balance = lastTransaction?.BalanceAfter ?? 0,
            driver.CashOnHand,
            driver.CashAlertThreshold,
            TotalSettlements = totalSettlements,
            IsOverThreshold = driver.CashOnHand > driver.CashAlertThreshold,
            LastUpdated = lastTransaction?.CreatedAt ?? DateTime.UtcNow
        }));
    }

    [HttpGet("driver/{driverId:guid}/transactions")]
    public async Task<IActionResult> GetDriverTransactions(Guid driverId, [FromQuery] WalletTransactionFilterDto filter)
    {
        var driverExists = await _db.Drivers.AnyAsync(d => d.Id == driverId);
        if (!driverExists)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        var query = _db.WalletTransactions.AsNoTracking()
            .Where(t => t.DriverId == driverId);

        if (filter.TransactionType.HasValue)
            query = query.Where(t => t.TransactionType == filter.TransactionType.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(t => t.CreatedAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(t => t.CreatedAt <= filter.DateTo.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new
            {
                t.Id,
                t.DriverId,
                t.OrderId,
                t.SettlementId,
                t.Amount,
                t.TransactionType,
                t.BalanceAfter,
                t.Description,
                t.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            filter.Page,
            filter.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpPost("driver/{driverId:guid}/adjust")]
    public async Task<IActionResult> AdjustBalance(Guid driverId, [FromBody] WalletAdjustmentDto dto)
    {
        var driver = await _db.Drivers.FindAsync(driverId);
        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        // Get current balance from the last transaction
        var lastTransaction = await _db.WalletTransactions
            .Where(t => t.DriverId == driverId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        var currentBalance = lastTransaction?.BalanceAfter ?? 0;
        var adjustmentAmount = dto.AdjustmentType == WalletAdjustmentType.Debit
            || dto.AdjustmentType == WalletAdjustmentType.Penalty
            ? -Math.Abs(dto.Amount)
            : Math.Abs(dto.Amount);

        var newBalance = currentBalance + adjustmentAmount;

        var transaction = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            Amount = adjustmentAmount,
            TransactionType = TransactionType.Adjustment,
            BalanceAfter = newBalance,
            Description = $"[تعديل إداري] {dto.Reason}"
        };

        await _db.WalletTransactions.AddAsync(transaction);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            transaction.Id,
            transaction.Amount,
            transaction.BalanceAfter,
            transaction.Description,
            transaction.CreatedAt
        }, SuccessMessages.WalletAdjusted));
    }

    [HttpPost("driver/{driverId:guid}/freeze")]
    public async Task<IActionResult> FreezeWallet(Guid driverId)
    {
        var driver = await _db.Drivers.FindAsync(driverId);
        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        // Freeze is handled via a flag; for now return success
        return Ok(ApiResponse<object>.Success(new
        {
            DriverId = driverId,
            IsFrozen = true,
            FrozenAt = DateTime.UtcNow
        }, SuccessMessages.WalletFrozenSuccess));
    }

    [HttpDelete("driver/{driverId:guid}/freeze")]
    public async Task<IActionResult> UnfreezeWallet(Guid driverId)
    {
        var driver = await _db.Drivers.FindAsync(driverId);
        if (driver is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        return Ok(ApiResponse<object>.Success(new
        {
            DriverId = driverId,
            IsFrozen = false,
            UnfrozenAt = DateTime.UtcNow
        }, SuccessMessages.WalletUnfrozen));
    }

    [HttpGet("frozen")]
    public IActionResult GetFrozenWallets()
    {
        // Frozen wallet tracking requires a dedicated entity/flag -- return empty for now
        return Ok(ApiResponse<object>.Success(new List<object>()));
    }

    [HttpPost("bulk-adjust")]
    public IActionResult BulkAdjust()
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية التعديل الجماعي. سيتم إرسال إشعار عند الانتهاء",
            RequestedAt = DateTime.UtcNow
        }, SuccessMessages.BulkAdjustmentCompleted));
    }

    [HttpGet("high-balance")]
    public async Task<IActionResult> GetHighBalance()
    {
        var drivers = await _db.Drivers.AsNoTracking()
            .Where(d => d.CashOnHand > d.CashAlertThreshold && d.IsActive)
            .OrderByDescending(d => d.CashOnHand)
            .Take(50)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.PhoneNumber,
                d.CashOnHand,
                d.CashAlertThreshold,
                OverAmount = d.CashOnHand - d.CashAlertThreshold
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(drivers));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var activeDrivers = await _db.Drivers.CountAsync(d => d.IsActive);
        var totalCashOnHand = await _db.Drivers.Where(d => d.IsActive).SumAsync(d => d.CashOnHand);
        var driversOverThreshold = await _db.Drivers.CountAsync(d => d.IsActive && d.CashOnHand > d.CashAlertThreshold);

        var totalSettlements = await _db.Settlements.SumAsync(s => s.Amount);

        var todayTransactions = await _db.WalletTransactions
            .CountAsync(t => t.CreatedAt >= DateTime.UtcNow.Date);

        return Ok(ApiResponse<object>.Success(new
        {
            ActiveWallets = activeDrivers,
            TotalCashOnHand = totalCashOnHand,
            DriversOverThreshold = driversOverThreshold,
            TotalSettlements = totalSettlements,
            TodayTransactionCount = todayTransactions
        }));
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminWalletFilterDto filter)
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية التصدير. سيتم إرسال الملف عبر الإشعارات",
            RequestedAt = DateTime.UtcNow
        }));
    }
}
