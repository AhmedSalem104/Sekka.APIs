using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "اسم جهة الطوارئ مطلوب")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "رقم التليفون مطلوب")]
    [Phone(ErrorMessage = "صيغة رقم التليفون غير صحيحة")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "صلة القرابة مطلوبة")]
    public string? Relationship { get; set; }
}
