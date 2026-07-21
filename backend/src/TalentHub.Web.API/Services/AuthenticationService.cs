using TalentHub.Web.API.Abstractions;
using TalentHub.Web.API.Models.Authentication;

namespace TalentHub.Web.API.Services;

public sealed class AuthenticationService(IJwtTokenService jwtTokenService, IRefreshTokenStore refreshTokenStore) : IAuthenticationService
{
    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = new UserResponse(
            Guid.NewGuid(),
            request.UserNameOrEmail,
            request.UserNameOrEmail.Contains('@') ? request.UserNameOrEmail : $"{request.UserNameOrEmail}@talenthub.local",
            ["User"],
            ["login"]);

        var refreshToken = await refreshTokenStore.IssueAsync(user.Id, cancellationToken).ConfigureAwait(false);
        var tokens = jwtTokenService.CreateToken(user, user.Claims);
        return tokens with { RefreshToken = refreshToken.Token };
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new UserResponse(
            Guid.NewGuid(),
            request.UserName,
            request.Email,
            ["User"],
            ["register"]);

        var refreshToken = await refreshTokenStore.IssueAsync(user.Id, cancellationToken).ConfigureAwait(false);
        var tokens = jwtTokenService.CreateToken(user, user.Claims);
        return tokens with { RefreshToken = refreshToken.Token };
    }

    public async Task<TokenResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var valid = await refreshTokenStore.ValidateAsync(request.RefreshToken, cancellationToken).ConfigureAwait(false);
        if (!valid)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        var userId = Guid.NewGuid();
        var user = new UserResponse(userId, "refresh.user", "refresh.user@talenthub.local", ["User"], ["refresh"]);
        var refreshToken = await refreshTokenStore.IssueAsync(user.Id, cancellationToken).ConfigureAwait(false);
        var tokens = jwtTokenService.CreateToken(user, user.Claims);
        return tokens with { RefreshToken = refreshToken.Token };
    }
}
