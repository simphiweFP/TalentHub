namespace TalentHub.Integration.Communication.Options;

public sealed class OAuthOptions
{
    public const string SectionName = "Communication:OAuth";

    public string? Authority { get; set; }

    public string? TokenEndpoint { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public string? Scope { get; set; }

    public string? StaticAccessToken { get; set; }
}
