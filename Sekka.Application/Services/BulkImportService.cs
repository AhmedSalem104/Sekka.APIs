using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class BulkImportService : IBulkImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BulkImportService> _logger;

    public BulkImportService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BulkImportService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<BulkImportResultDto>> ImportAsync(Guid driverId, BulkImportDto dto)
    {
        _logger.LogInformation("Bulk import requested by driver {DriverId}", driverId);

        if (string.IsNullOrWhiteSpace(dto.RawText))
            return Result<BulkImportResultDto>.BadRequest("النص المُدخل فارغ");

        var lines = dto.RawText
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var result = new BulkImportResultDto();
        var createdOrders = new List<Order>();

        foreach (var line in lines)
        {
            result.TotalParsed++;

            // Expected format: "name|phone|address|amount"
            var parts = line.Split('|');
            if (parts.Length < 4)
            {
                result.FailedImports++;
                result.Errors.Add($"سطر غير صالح (يجب 4 أجزاء مفصولة بـ |): \"{TruncateLine(line)}\"");
                continue;
            }

            var name = parts[0].Trim();
            var phone = parts[1].Trim();
            var address = parts[2].Trim();
            var amountText = parts[3].Trim();

            if (!decimal.TryParse(amountText, out var amount))
            {
                result.FailedImports++;
                result.Errors.Add($"مبلغ غير صالح في السطر: \"{TruncateLine(line)}\"");
                continue;
            }

            if (string.IsNullOrEmpty(address))
            {
                result.FailedImports++;
                result.Errors.Add($"عنوان فارغ في السطر: \"{TruncateLine(line)}\"");
                continue;
            }

            var normalizedPhone = !string.IsNullOrEmpty(phone) ? EgyptianPhoneHelper.Normalize(phone) : null;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                DriverId = driverId,
                OrderNumber = $"BLK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                CustomerName = string.IsNullOrEmpty(name) ? null : name,
                CustomerPhone = normalizedPhone,
                DeliveryAddress = address,
                Amount = amount,
                PaymentMethod = dto.DefaultPaymentMethod,
                Status = OrderStatus.Pending,
                Priority = OrderPriority.Normal,
                SourceType = OrderSourceType.BulkImport,
                PartnerId = dto.PartnerId,
                ItemCount = 1
            };

            await orderRepo.AddAsync(order);
            createdOrders.Add(order);
            result.SuccessfulImports++;
        }

        if (createdOrders.Count > 0)
            await _unitOfWork.SaveChangesAsync();

        result.Orders = _mapper.Map<List<OrderDto>>(createdOrders);

        _logger.LogInformation("Bulk import completed for driver {DriverId}: {Success}/{Total} imported",
            driverId, result.SuccessfulImports, result.TotalParsed);

        return Result<BulkImportResultDto>.Success(result);
    }

    private static string TruncateLine(string line)
        => line.Length > 80 ? line[..80] + "..." : line;
}
