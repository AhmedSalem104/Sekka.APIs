namespace Sekka.Core.DTOs.Profile;

public class ProfileCompletionDto
{
    public int CompletionPercentage { get; set; }
    public List<string> CompletedSteps { get; set; } = new();
    public List<ProfileStepDto> PendingSteps { get; set; } = new();
    public bool IsProfileComplete { get; set; }
}

public class ProfileStepDto
{
    public string StepName { get; set; } = null!;
    public string StepKey { get; set; } = null!;
    public bool IsRequired { get; set; }
    public int Weight { get; set; }
}
