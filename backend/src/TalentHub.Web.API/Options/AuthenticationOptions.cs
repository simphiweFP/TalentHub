namespace TalentHub.Web.API.Options;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public JwtOptions Jwt { get; set; } = new();

    public RefreshTokenOptions RefreshTokens { get; set; } = new();

    public ApiKeyOptions ApiKey { get; set; } = new();
}
