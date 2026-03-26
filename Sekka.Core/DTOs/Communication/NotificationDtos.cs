namespace Sekka.Core.DTOs.Communication;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string NotificationType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
    public string? ActionType { get; set; }
    public string? ActionData { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BroadcastNotificationDto
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Priority { get; set; }
    public string? TargetRegion { get; set; }
}

public class SendToDriverDto
{
    public Guid DriverId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Priority { get; set; }
}

public class AdminNotificationDto : NotificationDto
{
    public string DriverName { get; set; } = null!;
}
