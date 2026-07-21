namespace TalentHub.Integration.Communication.Models;

public sealed record ProviderCompany(
    string ExternalId,
    string Name,
    string? WebsiteUrl,
    string? Description,
    string? Location,
    bool IsActive = true);
