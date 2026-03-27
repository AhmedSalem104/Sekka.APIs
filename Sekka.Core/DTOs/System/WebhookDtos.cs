namespace Sekka.Core.DTOs.System;

// ── Create ──
public class CreateWebhookConfigDto
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public Guid? PartnerId { get; set; }
    public List<string> Events { get; set; } = new();
}

// ── Update ──
public class UpdateWebhookConfigDto
{
    public string? Name { get; set; }
    public string? Url { get; set; }
    public List<string>? Events { get; set; }
    public bool? IsActive { get; set; }
}

// ── Response ──
public class WebhookConfigDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? PartnerName { get; set; }
    public List<string> Events { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? LastTriggeredAt { get; set; }
    public int FailureCount { get; set; }
}

// ── Log ──
public class WebhookLogDto
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public int? ResponseStatusCode { get; set; }
    public int RetryCount { get; set; }
    public DateTime SentAt { get; set; }
}
