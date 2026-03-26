using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class AddressSwapService : IAddressSwapService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddressSwapService> _logger;

    public AddressSwapService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddressSwapService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AddressSwapLogDto>> SwapAddressAsync(Guid driverId, Guid orderId, SwapAddressDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<AddressSwapLogDto>.NotFound(ErrorMessages.OrderNotFound);

        var log = new AddressSwapLog
        {
            OrderId = orderId,
            OldAddress = order.DeliveryAddress,
            OldLatitude = order.DeliveryLatitude,
            OldLongitude = order.DeliveryLongitude,
            NewAddress = dto.NewAddress,
            NewLatitude = dto.NewLatitude,
            NewLongitude = dto.NewLongitude,
            Reason = dto.Reason,
            SwappedAt = DateTime.UtcNow
        };

        // Calculate distance difference if coordinates are available
        if (order.DeliveryLatitude.HasValue && order.DeliveryLongitude.HasValue
            && dto.NewLatitude.HasValue && dto.NewLongitude.HasValue)
        {
            log.DistanceDifferenceKm = CalculateDistanceKm(
                order.DeliveryLatitude.Value, order.DeliveryLongitude.Value,
                dto.NewLatitude.Value, dto.NewLongitude.Value);
        }

        // Update the order address
        order.DeliveryAddress = dto.NewAddress;
        order.DeliveryLatitude = dto.NewLatitude;
        order.DeliveryLongitude = dto.NewLongitude;
        order.UpdatedAt = DateTime.UtcNow;
        repo.Update(order);

        var logRepo = _unitOfWork.GetRepository<AddressSwapLog, Guid>();
        await logRepo.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Address swapped for order {OrderId} by driver {DriverId}", orderId, driverId);

        return Result<AddressSwapLogDto>.Success(_mapper.Map<AddressSwapLogDto>(log));
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(R * c, 2);
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
