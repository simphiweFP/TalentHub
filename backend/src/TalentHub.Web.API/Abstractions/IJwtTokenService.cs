using TalentHub.Web.API.Models.Authentication;

namespace TalentHub.Web.API.Abstractions;

public interface IJwtTokenService
{
    TokenResponse CreateToken(UserResponse user, IReadOnlyCollection<string> claims);
}
