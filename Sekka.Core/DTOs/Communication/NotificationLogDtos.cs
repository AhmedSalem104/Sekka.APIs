namespace Sekka.Core.DTOs.Communication;

public class NotificationLogDto
{
    public long Id { get; set; }
    public string Channel { get; set; } = null!;
    public string? Subject { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; }
}
