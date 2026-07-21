namespace TalentHub.Integration.RemoteOK.Models;

public sealed record RemoteOkSearchRequest(
    string? SearchTerm = null,
    string? Location = null,
    int PageNumber = 1,
    int PageSize = 20);
