namespace TalentHub.Integration.Communication.Models;

public sealed record ProviderJob(
    string ExternalId,
    string Title,
    string CompanyName,
    string? Location,
    string? Url,
    DateTimeOffset PublishedAtUtc,
    bool IsRemote);
