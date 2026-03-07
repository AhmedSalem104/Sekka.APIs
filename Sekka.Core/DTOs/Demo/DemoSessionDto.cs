namespace Sekka.Core.DTOs.Demo;

public class DemoSessionDto
{
    public Guid SessionId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid DemoDriverId { get; set; }
    public string Token { get; set; } = null!;
}

public class DemoDataDto
{
    public List<object> Orders { get; set; } = new();
    public List<object> Customers { get; set; } = new();
    public List<object> Partners { get; set; } = new();
    public object? DailyStats { get; set; }
    public int RemainingMinutes { get; set; }
}
