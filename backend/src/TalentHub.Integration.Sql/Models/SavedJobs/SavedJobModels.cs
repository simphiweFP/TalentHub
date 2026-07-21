using TalentHub.Integration.Sql.Common;

namespace TalentHub.Integration.Sql.Models.SavedJobs;

public sealed record SavedJobQueryOptions(
    Guid? UserId = null,
    Guid? JobId = null,
    string? SearchTerm = null,
    string SortBy = "SavedAtUtc",
    SortDirection SortDirection = SortDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20);

public sealed record SavedJobReadModel
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
    public string JobTitle { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public DateTime SavedAtUtc { get; init; }
}

public sealed record SavedJobCommand(Guid UserId, Guid JobId);
