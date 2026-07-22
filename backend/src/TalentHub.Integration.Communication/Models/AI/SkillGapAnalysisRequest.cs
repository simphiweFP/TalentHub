namespace TalentHub.Integration.Communication.Models;

public sealed record SkillGapAnalysisRequest(
    IReadOnlyCollection<string> CurrentSkills,
    IReadOnlyCollection<string> TargetSkills,
    string? Context = null);
