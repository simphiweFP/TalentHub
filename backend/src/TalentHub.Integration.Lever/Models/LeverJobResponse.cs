namespace TalentHub.Integration.Lever.Models;

public sealed record LeverJobResponse
{
    public string Id { get; init; } = string.Empty;

    public string Text { get; init; } = string.Empty;

    public string Lever { get; init; } = string.Empty;

    public string? Categories { get; init; }

    public string? Commitment { get; init; }

    public string? Location { get; init; }

    public string? Url { get; init; }

    public string? Description { get; init; }

    public bool Remote { get; init; }

    public DateTimeOffset? CreatedAt { get; init; }
}
