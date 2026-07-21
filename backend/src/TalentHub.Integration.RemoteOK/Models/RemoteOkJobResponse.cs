namespace TalentHub.Integration.RemoteOK.Models;

public sealed record RemoteOkJobResponse
{
    public string Id { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    public string? Location { get; init; }
    public string? Url { get; init; }
    public string? Description { get; init; }
    public bool Remote { get; init; }
    public DateTimeOffset? Date { get; init; }
}
