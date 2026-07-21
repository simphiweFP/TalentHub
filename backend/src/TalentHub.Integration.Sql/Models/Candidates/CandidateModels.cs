using TalentHub.Integration.Sql.Common;

namespace TalentHub.Integration.Sql.Models.Candidates;

public sealed record CandidateQueryOptions(
    Guid? LocationId = null,
    bool? IsOpenToWork = null,
    string? SearchTerm = null,
    string SortBy = "CreatedAtUtc",
    SortDirection SortDirection = SortDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20);

public sealed record CandidateReadModel
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

public sealed record CandidateCommand(
    Guid UserId,
    string? Headline,
    string? Summary,
    string? ResumeUrl,
    decimal? YearsOfExperience,
    Guid? LocationId,
    bool IsOpenToWork);
