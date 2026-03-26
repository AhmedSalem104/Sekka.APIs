using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Order;

public class DeliveryAttemptDto
{
    public Guid Id { get; set; }
    public int AttemptNumber { get; set; }
    public DeliveryFailReason Status { get; set; }
    public string? Reason { get; set; }
    public bool AutoMessageSent { get; set; }
    public DateTime Timestamp { get; set; }
}

public class CancellationLogDto
{
    public Guid Id { get; set; }
    public CancellationReason Reason { get; set; }
    public string? ReasonText { get; set; }
    public decimal LossAmount { get; set; }
    public double? DistanceTravelledKm { get; set; }
    public int? TimeLostMinutes { get; set; }
    public decimal? FuelCostLost { get; set; }
    public DateTime CancelledAt { get; set; }
}

public class OrderPhotoDto
{
    public Guid Id { get; set; }
    public string PhotoUrl { get; set; } = null!;
    public PhotoType PhotoType { get; set; }
    public DateTime TakenAt { get; set; }
}

public class OrderSourceTagDto
{
    public SourceTagType SourceType { get; set; }
    public string? SourceName { get; set; }
    public string? SourceReference { get; set; }
}

public class WaitingTimerDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Reason { get; set; }
}

public class AddressSwapLogDto
{
    public Guid Id { get; set; }
    public string OldAddress { get; set; } = null!;
    public string NewAddress { get; set; } = null!;
    public double? DistanceDifferenceKm { get; set; }
    public int? TimeDifferenceMinutes { get; set; }
    public decimal? CostDifference { get; set; }
    public DateTime SwappedAt { get; set; }
}

public class SwapAddressDto
{
    public string NewAddress { get; set; } = null!;
    public double? NewLatitude { get; set; }
    public double? NewLongitude { get; set; }
    public string? Reason { get; set; }
}

public class OrderTransferResponseDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string FromDriverName { get; set; } = null!;
    public string? ToDriverName { get; set; }
    public string? DeepLinkToken { get; set; }
    public TransferStatus Status { get; set; }
    public DateTime TransferredAt { get; set; }
}
