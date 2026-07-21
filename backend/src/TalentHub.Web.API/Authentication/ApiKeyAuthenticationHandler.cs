using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TalentHub.Web.API.Options;
using TalentHub.Web.API.Security;
using AuthOptions = TalentHub.Web.API.Options.AuthenticationOptions;

namespace TalentHub.Web.API.Authentication;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<AuthOptions> authenticationOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly AuthOptions _authenticationOptions = authenticationOptions.Value;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_authenticationOptions.ApiKey.Enabled)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (string.IsNullOrWhiteSpace(_authenticationOptions.ApiKey.Value))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Request.Headers.TryGetValue(_authenticationOptions.ApiKey.HeaderName, out var apiKeyValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var apiKey = apiKeyValues.ToString();
        if (!string.Equals(apiKey, _authenticationOptions.ApiKey.Value, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "api-key-client"),
            new Claim(ClaimTypes.Name, "api-key-client"),
            new Claim(ClaimNames.Role, "ApiKey")
        };

        var identity = new ClaimsIdentity(claims, AuthenticationSchemeNames.ApiKey);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationSchemeNames.ApiKey);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
