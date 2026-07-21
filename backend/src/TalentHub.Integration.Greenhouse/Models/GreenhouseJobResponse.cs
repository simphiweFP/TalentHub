namespace TalentHub.Integration.Greenhouse.Models;

public sealed record GreenhouseJobResponse
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string? Location { get; init; }
    public string? Url { get; init; }
    public string? Content { get; init; }
    public bool IsRemote { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
}
