using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Account;

public class DeleteAccountDto
{
    public string? Reason { get; set; }
}

public class ConfirmDeletionDto
{
    public string OtpCode { get; set; } = null!;
}

public class AccountDeletionRequestDto
{
    public Guid Id { get; set; }
    public DeletionRequestStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ActiveSessionDto
{
    public Guid Id { get; set; }
    public string? DeviceName { get; set; }
    public DevicePlatform DevicePlatform { get; set; }
    public string? IpAddress { get; set; }
    public DateTime LastActiveAt { get; set; }
    public bool IsCurrentSession { get; set; }
    public DateTime CreatedAt { get; set; }
}
