using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentHub.Web.API.Abstractions;
using TalentHub.Web.API.Models.Authentication;

namespace TalentHub.Web.API.Controllers;

public sealed class AuthController(IAuthenticationService authenticationService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authenticationService.LoginAsync(request, cancellationToken));

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        => Ok(await authenticationService.RegisterAsync(request, cancellationToken));

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        => Ok(await authenticationService.RefreshAsync(request, cancellationToken));
}
