using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Handlers;

public sealed class OAuthHandler(IOAuthTokenProvider tokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token?.AccessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token.TokenType ?? "Bearer", token.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
