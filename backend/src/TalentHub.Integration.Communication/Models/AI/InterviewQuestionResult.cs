namespace TalentHub.Integration.Communication.Models;

public sealed record InterviewQuestionResult
{
    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<string> Questions { get; init; } = [];
}
