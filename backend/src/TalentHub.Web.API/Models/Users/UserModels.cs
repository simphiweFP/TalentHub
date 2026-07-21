using System.ComponentModel.DataAnnotations;
using TalentHub.Web.API.Models.Common;

namespace TalentHub.Web.API.Models.Users;

public sealed record UserQueryParameters : PagedQueryParameters
{
    public bool? IsActive { get; init; }
}

public sealed record UserResponse
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

public sealed record CreateUserRequest(
    [property: Required, MaxLength(100)] string UserName,
    [property: Required, MaxLength(256), EmailAddress] string Email,
    [property: MaxLength(100)] string? FirstName,
    [property: MaxLength(100)] string? LastName,
    [property: MaxLength(200)] string? DisplayName,
    [property: MaxLength(30)] string? PhoneNumber,
    bool IsActive = true);

public sealed record UpdateUserRequest(
    [property: Required, MaxLength(100)] string UserName,
    [property: Required, MaxLength(256), EmailAddress] string Email,
    [property: MaxLength(100)] string? FirstName,
    [property: MaxLength(100)] string? LastName,
    [property: MaxLength(200)] string? DisplayName,
    [property: MaxLength(30)] string? PhoneNumber,
    bool IsActive);
