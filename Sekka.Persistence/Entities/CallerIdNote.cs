using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CallerIdNote : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public Guid? CustomerId { get; set; }
    public Guid? PartnerId { get; set; }
    public ContactType ContactType { get; set; }
    public string? DisplayName { get; set; }
    public string? Note { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Customer? Customer { get; set; }
    public Partner? Partner { get; set; }
}
