using System.ComponentModel.DataAnnotations;
using TalentHub.Web.API.Models.Common;

namespace TalentHub.Web.API.Models.SavedJobs;

public sealed record SavedJobQueryParameters : PagedQueryParameters
{
    public Guid? UserId { get; init; }

    public Guid? JobId { get; init; }
}

public sealed record SavedJobResponse
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public Guid JobId { get; init; }

    public string JobTitle { get; init; } = string.Empty;

    public string CompanyName { get; init; } = string.Empty;

    public DateTime SavedAtUtc { get; init; }
}

public sealed record CreateSavedJobRequest(
    [property: Required] Guid UserId,
    [property: Required] Guid JobId);
