using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TalentHub.Web.API.Abstractions;
using TalentHub.Web.API.Models.Authentication;
using TalentHub.Web.API.Options;
using TalentHub.Web.API.Security;

namespace TalentHub.Web.API.Services;

public sealed class JwtTokenService(IOptions<AuthenticationOptions> options) : IJwtTokenService
{
    private readonly AuthenticationOptions _options = options.Value;

    public TokenResponse CreateToken(UserResponse user, IReadOnlyCollection<string> claims)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_options.Jwt.AccessTokenMinutes);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Jwt.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var identityClaims = new List<Claim>
        {
            new(ClaimNames.UserId, user.Id.ToString()),
            new(ClaimNames.UserName, user.UserName),
            new(ClaimNames.Email, user.Email)
        };

        identityClaims.AddRange(user.Roles.Select(role => new Claim(ClaimNames.Role, role)));
        identityClaims.AddRange(claims.Select(claim => new Claim(claim, "true")));

        var token = new JwtSecurityToken(
            issuer: _options.Jwt.Issuer,
            audience: _options.Jwt.Audience,
            claims: identityClaims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(token);
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return new TokenResponse(accessToken, refreshToken, expiresAt);
    }
}
