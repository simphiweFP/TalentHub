namespace TalentHub.Integration.Communication.Models;

public sealed record ResumeAnalysisRequest(
    string ResumeText,
    string? TargetRole = null,
    string? JobDescription = null,
    IReadOnlyCollection<string>? RequiredSkills = null);
