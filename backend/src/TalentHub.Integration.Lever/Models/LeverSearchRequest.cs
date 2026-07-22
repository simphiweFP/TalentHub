namespace TalentHub.Integration.Lever.Models;

public sealed record LeverSearchRequest(
    string? SearchTerm = null,
    string? Location = null,
    int PageNumber = 1,
    int PageSize = 20);
