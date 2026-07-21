using TalentHub.Web.API.Models.Authentication;

namespace TalentHub.Web.API.Abstractions;

public interface IAuthenticationService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<TokenResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
