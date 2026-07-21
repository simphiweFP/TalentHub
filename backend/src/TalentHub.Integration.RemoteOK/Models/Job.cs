namespace TalentHub.Integration.RemoteOK.Models;

public sealed record Job
{
    public string Source { get; init; } = "RemoteOK";

    public string ExternalId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string CompanyName { get; init; } = string.Empty;

    public string? CompanyWebsite { get; init; }

    public string? Location { get; init; }

    public string? Url { get; init; }

    public string? Description { get; init; }

    public bool IsRemote { get; init; }

    public DateTimeOffset PublishedAtUtc { get; init; }
}
