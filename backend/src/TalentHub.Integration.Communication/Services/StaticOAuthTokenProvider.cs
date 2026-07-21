using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Services;

public sealed class StaticOAuthTokenProvider(IOptions<OAuthOptions> options) : IOAuthTokenProvider
{
    private readonly OAuthOptions _options = options.Value;

    public Task<OAuthTokenResponse?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.StaticAccessToken))
        {
            return Task.FromResult<OAuthTokenResponse?>(null);
        }

        return Task.FromResult<OAuthTokenResponse?>(new OAuthTokenResponse(_options.StaticAccessToken, DateTimeOffset.UtcNow.AddHours(1), "Bearer", _options.Scope));
    }
}
