using TalentHub.Web.API.Identity;

namespace TalentHub.Web.API.Abstractions;

public interface IRefreshTokenStore
{
    Task<RefreshTokenRecord> IssueAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}
