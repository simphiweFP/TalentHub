using System.ComponentModel.DataAnnotations;
using TalentHub.Web.API.Models.Common;

namespace TalentHub.Web.API.Models.Candidates;

public sealed record CandidateQueryParameters : PagedQueryParameters
{
    public Guid? LocationId { get; init; }

    public bool? IsOpenToWork { get; init; }
}

public sealed record CandidateResponse
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string? Headline { get; init; }

    public string? Summary { get; init; }

    public string? ResumeUrl { get; init; }

    public decimal? YearsOfExperience { get; init; }

    public Guid? LocationId { get; init; }

    public bool IsOpenToWork { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed record CreateCandidateRequest(
    [property: Required] Guid UserId,
    [property: MaxLength(250)] string? Headline,
    string? Summary,
    [property: MaxLength(500)] string? ResumeUrl,
    decimal? YearsOfExperience,
    Guid? LocationId,
    bool IsOpenToWork = false);

public sealed record UpdateCandidateRequest(
    [property: Required] Guid UserId,
    [property: MaxLength(250)] string? Headline,
    string? Summary,
    [property: MaxLength(500)] string? ResumeUrl,
    decimal? YearsOfExperience,
    Guid? LocationId,
    bool IsOpenToWork);
