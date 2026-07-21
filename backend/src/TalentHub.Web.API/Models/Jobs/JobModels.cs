using System.ComponentModel.DataAnnotations;
using TalentHub.Web.API.Models.Common;

namespace TalentHub.Web.API.Models.Jobs;

public sealed record JobQueryParameters : PagedQueryParameters
{
    public Guid? CompanyId { get; init; }

    public int? JobCategoryId { get; init; }

    public Guid? LocationId { get; init; }

    public bool? IsRemote { get; init; }

    public bool? IsActive { get; init; }
}

public sealed record JobResponse
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

public sealed record CreateJobRequest(
    [property: Required, MaxLength(200)] string Title,
    [property: Required] Guid CompanyId,
    [property: Required] int JobCategoryId,
    Guid? LocationId,
    [property: MaxLength(int.MaxValue)] string? Description,
    int EmploymentType,
    int WorkMode,
    int SeniorityLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    [property: MaxLength(3)] string? CurrencyCode,
    bool IsRemote,
    bool IsActive = true);

public sealed record UpdateJobRequest(
    [property: Required, MaxLength(200)] string Title,
    [property: Required] Guid CompanyId,
    [property: Required] int JobCategoryId,
    Guid? LocationId,
    [property: MaxLength(int.MaxValue)] string? Description,
    int EmploymentType,
    int WorkMode,
    int SeniorityLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    [property: MaxLength(3)] string? CurrencyCode,
    bool IsRemote,
    bool IsActive);
