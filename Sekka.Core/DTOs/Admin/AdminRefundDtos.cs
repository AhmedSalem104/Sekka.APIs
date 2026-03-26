using Sekka.Core.DTOs.Wallet;

namespace Sekka.Core.DTOs.Admin;

public class AdminRefundDto : RefundDto
{
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
}
