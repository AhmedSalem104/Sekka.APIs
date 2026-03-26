using Sekka.Core.DTOs.Wallet;

namespace Sekka.Core.DTOs.Admin;

public class AdminDisputeDto : DisputeDto
{
    public string DriverName { get; set; } = null!;
    public string DriverPhone { get; set; } = null!;
}
