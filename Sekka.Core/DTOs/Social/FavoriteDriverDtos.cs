using System.ComponentModel.DataAnnotations;

namespace Sekka.Core.DTOs.Social;

public class FavoriteDriverDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public Guid? LinkedDriverId { get; set; }
    public bool IsAppUser { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddFavoriteDriverDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    public string Phone { get; set; } = null!;
}
