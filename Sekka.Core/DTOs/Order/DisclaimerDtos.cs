namespace Sekka.Core.DTOs.Order;

public class CreateDisclaimerDto
{
    public string ItemsDescription { get; set; } = null!;
    public string Condition { get; set; } = null!;
    public bool CustomerAcknowledged { get; set; }
    public List<Guid>? PhotoIds { get; set; }
    public string? Notes { get; set; }
}

public class DisclaimerDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ItemsDescription { get; set; } = null!;
    public string Condition { get; set; } = null!;
    public bool CustomerAcknowledged { get; set; }
    public List<OrderPhotoDto> Photos { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
