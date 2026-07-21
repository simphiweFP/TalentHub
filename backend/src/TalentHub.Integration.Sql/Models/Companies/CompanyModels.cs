using TalentHub.Integration.Sql.Common;

namespace TalentHub.Integration.Sql.Models.Companies;

public sealed record CompanyQueryOptions(
    bool? IsActive = null,
    string? Industry = null,
    string? SearchTerm = null,
    string SortBy = "CreatedAtUtc",
    SortDirection SortDirection = SortDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20);

public sealed record CompanyReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? WebsiteUrl { get; init; }
    public string? Description { get; init; }
    public string? Industry { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed record CompanyCommand(
    string Name,
    string Slug,
    string? WebsiteUrl,
    string? Description,
    string? Industry,
    bool IsActive);
