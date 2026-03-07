namespace Sekka.Core.DTOs.Profile;

public class EmergencyContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Relationship { get; set; }
}

public class CreateEmergencyContactDto
{
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Relationship { get; set; }
}
