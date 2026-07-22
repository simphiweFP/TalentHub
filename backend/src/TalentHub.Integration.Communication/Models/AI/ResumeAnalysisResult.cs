namespace TalentHub.Integration.Communication.Models;

public sealed record ResumeAnalysisResult
{
    public string Summary { get; init; } = string.Empty;

    public int MatchScore { get; init; }

    public IReadOnlyList<string> Strengths { get; init; } = [];

    public IReadOnlyList<string> Gaps { get; init; } = [];

    public IReadOnlyList<string> RecommendedSkills { get; init; } = [];
}
