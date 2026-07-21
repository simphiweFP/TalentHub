using TalentHub.Integration.Sql.Common;
using TalentHub.Integration.Sql.Pagination;

namespace TalentHub.Integration.Sql.Models.Jobs;

public sealed record JobQueryOptions(
    Guid? CompanyId = null,
    int? JobCategoryId = null,
    Guid? LocationId = null,
    bool? IsRemote = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    string SortBy = "CreatedAtUtc",
    SortDirection SortDirection = SortDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20);

public sealed record JobReadModel
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public int JobCategoryId { get; init; }
    public Guid? LocationId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int EmploymentType { get; init; }
    public int WorkMode { get; init; }
    public int SeniorityLevel { get; init; }
    public decimal? SalaryMin { get; init; }
    public decimal? SalaryMax { get; init; }
    public string? CurrencyCode { get; init; }
    public bool IsRemote { get; init; }
    public bool IsActive { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string JobCategoryName { get; init; } = string.Empty;
    public string? LocationCity { get; init; }
    public string? LocationCountry { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed record JobCommand(
    Guid CompanyId,
    int JobCategoryId,
    Guid? LocationId,
    string Title,
    string Slug,
    string? Description,
    int EmploymentType,
    int WorkMode,
    int SeniorityLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? CurrencyCode,
    bool IsRemote,
    bool IsActive);
