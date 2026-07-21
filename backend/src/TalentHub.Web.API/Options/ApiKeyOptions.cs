namespace TalentHub.Web.API.Options;

public sealed class ApiKeyOptions
{
    public string HeaderName { get; set; } = "X-API-Key";

    public string? Value { get; set; } = "change-me-api-key";

    public bool Enabled { get; set; } = true;
}
