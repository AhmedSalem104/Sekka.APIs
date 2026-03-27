namespace Sekka.Core.DTOs.Social;

public class OmniSearchResultDto
{
    public List<object> Orders { get; set; } = new();
    public List<object> Customers { get; set; } = new();
    public List<object> Partners { get; set; } = new();
    public int TotalResults { get; set; }
}
