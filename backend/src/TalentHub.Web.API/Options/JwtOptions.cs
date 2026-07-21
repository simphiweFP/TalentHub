namespace TalentHub.Web.API.Options;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "TalentHub";

    public string Audience { get; set; } = "TalentHub";

    public string SigningKey { get; set; } = "change-me-change-me-change-me-change-me";

    public int AccessTokenMinutes { get; set; } = 60;
}
