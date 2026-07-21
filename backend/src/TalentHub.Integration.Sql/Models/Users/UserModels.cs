using TalentHub.Integration.Sql.Common;

namespace TalentHub.Integration.Sql.Models.Users;

public sealed record UserQueryOptions(
    bool? IsActive = null,
    string? SearchTerm = null,
    string SortBy = "CreatedAtUtc",
    SortDirection SortDirection = SortDirection.Descending,
    int PageNumber = 1,
    int PageSize = 20);

public sealed record UserReadModel
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? DisplayName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed record UserCommand(
    string UserName,
    string Email,
    string? FirstName,
    string? LastName,
    string? DisplayName,
    string? PhoneNumber,
    bool IsActive);
