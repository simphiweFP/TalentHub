using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Communication.Abstractions;

public interface IOAuthTokenProvider
{
    Task<OAuthTokenResponse?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
