using Microsoft.Extensions.Options;
using TalentHub.Web.API.Abstractions;
using TalentHub.Web.API.Identity;
using TalentHub.Web.API.Models.Authentication;
using TalentHub.Web.API.Options;
using TalentHub.Web.API.Services;

namespace TalentHub.Tests.Authentication;

public sealed class AuthenticationTests
{
    [Fact]
    public void JwtTokenService_creates_access_and_refresh_tokens()
    {
        // Arrange
        var service = new JwtTokenService(Options.Create(CreateAuthenticationOptions()));
        var user = new UserResponse(Guid.NewGuid(), "demo", "demo@talenthub.local", ["User"], ["login"]);

        // Act
        var token = service.CreateToken(user, user.Claims);

        // Assert
        Assert.NotEmpty(token.AccessToken);
        Assert.NotEmpty(token.RefreshToken);
        Assert.True(token.ExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task AuthenticationService_login_register_and_refresh_issue_tokens()
    {
        // Arrange
        var tokenService = new JwtTokenService(Options.Create(CreateAuthenticationOptions()));
        var refreshTokenStore = new InMemoryRefreshTokenStore(Options.Create(CreateAuthenticationOptions()));
        var service = new AuthenticationService(tokenService, refreshTokenStore);

        // Act
        var login = await service.LoginAsync(new LoginRequest("demo", "password")).ConfigureAwait(false);
        var register = await service.RegisterAsync(new RegisterRequest("demo", "demo@talenthub.local", "password")).ConfigureAwait(false);
        var refresh = await service.RefreshAsync(new RefreshTokenRequest(login.AccessToken, login.RefreshToken)).ConfigureAwait(false);

        // Assert
        Assert.NotEmpty(login.AccessToken);
        Assert.NotEmpty(register.AccessToken);
        Assert.NotEmpty(refresh.AccessToken);
        Assert.NotEmpty(refresh.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenStore_issue_validate_and_revoke_work_as_expected()
    {
        // Arrange
        var store = new InMemoryRefreshTokenStore(Options.Create(CreateAuthenticationOptions()));

        // Act
        var issued = await store.IssueAsync(Guid.NewGuid()).ConfigureAwait(false);
        var validBeforeRevoke = await store.ValidateAsync(issued.Token).ConfigureAwait(false);
        await store.RevokeAsync(issued.Token).ConfigureAwait(false);
        var validAfterRevoke = await store.ValidateAsync(issued.Token).ConfigureAwait(false);

        // Assert
        Assert.True(validBeforeRevoke);
        Assert.False(validAfterRevoke);
    }

    private static AuthenticationOptions CreateAuthenticationOptions()
        => new()
        {
            Jwt = new JwtOptions
            {
                Issuer = "TalentHub",
                Audience = "TalentHub",
                SigningKey = "change-me-change-me-change-me-change-me",
                AccessTokenMinutes = 60
            },
            RefreshTokens = new RefreshTokenOptions
            {
                ExpirationDays = 7,
                StoreInMemory = true
            },
            ApiKey = new ApiKeyOptions
            {
                Enabled = true,
                HeaderName = "X-API-Key",
                Value = "change-me"
            }
        };
}
