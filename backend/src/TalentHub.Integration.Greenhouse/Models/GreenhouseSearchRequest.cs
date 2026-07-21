namespace TalentHub.Integration.Greenhouse.Models;

public sealed record GreenhouseSearchRequest(
    string? SearchTerm = null,
    string? Location = null,
    int PageNumber = 1,
    int PageSize = 20);
