using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class FavoriteDriver : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public Guid? LinkedDriverId { get; set; }
    public bool IsAppUser { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Driver? LinkedDriver { get; set; }
}
