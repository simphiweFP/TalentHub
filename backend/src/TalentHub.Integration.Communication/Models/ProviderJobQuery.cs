namespace TalentHub.Integration.Communication.Models;

public sealed record ProviderJobQuery(
    string? SearchTerm = null,
    string? Location = null,
    int PageNumber = 1,
    int PageSize = 20);
