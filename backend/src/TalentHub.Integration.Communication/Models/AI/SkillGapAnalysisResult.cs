namespace TalentHub.Integration.Communication.Models;

public sealed record SkillGapAnalysisResult
{
    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<string> MatchedSkills { get; init; } = [];

    public IReadOnlyList<string> MissingSkills { get; init; } = [];

    public IReadOnlyList<string> RecommendedLearningPaths { get; init; } = [];
}
