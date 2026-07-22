namespace TalentHub.Integration.Communication.Models;

public sealed record JobMatchResult
{
    public string Summary { get; init; } = string.Empty;

    public int MatchScore { get; init; }

    public IReadOnlyList<string> MatchingFactors { get; init; } = [];

    public IReadOnlyList<string> Concerns { get; init; } = [];

    public IReadOnlyList<string> SuggestedActions { get; init; } = [];
}
