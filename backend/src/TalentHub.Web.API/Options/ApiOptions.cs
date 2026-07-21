namespace TalentHub.Web.API.Options;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public string Name { get; set; } = "TalentHub API";

    public string Description { get; set; } = "Backend foundation for TalentHub.";

    public string Version { get; set; } = "1.0";
}
