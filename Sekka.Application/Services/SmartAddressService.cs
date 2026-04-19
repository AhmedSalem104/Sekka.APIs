using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class SmartAddressService : ISmartAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SmartAddressService> _logger;

    public SmartAddressService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SmartAddressService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<SmartAddressDto>> GetSmartAddressAsync(string rawAddress)
    {
        return Task.FromResult(Result<SmartAddressDto>.NotImplemented(ErrorMessages.FeatureUnderDevelopment("العنوان الذكي")));
    }

    public Task<Result<List<SmartAddressDto>>> SearchAddressAsync(string query, double? nearLatitude, double? nearLongitude)
    {
        return Task.FromResult(Result<List<SmartAddressDto>>.NotImplemented(ErrorMessages.FeatureUnderDevelopment("البحث عن عنوان")));
    }

    public Task<Result<SmartAddressDto>> GeocodeAsync(double latitude, double longitude)
    {
        return Task.FromResult(Result<SmartAddressDto>.NotImplemented(ErrorMessages.FeatureUnderDevelopment("تحويل الإحداثيات")));
    }
}
