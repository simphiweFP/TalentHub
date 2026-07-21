using System.ComponentModel.DataAnnotations;
using TalentHub.Web.API.Models.Common;

namespace TalentHub.Web.API.Models.Companies;

public sealed record CompanyQueryParameters : PagedQueryParameters
{
    public bool? IsActive { get; init; }

    public string? Industry { get; init; }
}

public sealed record CompanyResponse
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

public sealed record CreateCompanyRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: Required, MaxLength(200)] string Slug,
    [property: MaxLength(500)] string? WebsiteUrl,
    string? Description,
    [property: MaxLength(150)] string? Industry,
    bool IsActive = true);

public sealed record UpdateCompanyRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: Required, MaxLength(200)] string Slug,
    [property: MaxLength(500)] string? WebsiteUrl,
    string? Description,
    [property: MaxLength(150)] string? Industry,
    bool IsActive);
