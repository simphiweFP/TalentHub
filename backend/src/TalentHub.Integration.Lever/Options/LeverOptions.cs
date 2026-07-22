namespace TalentHub.Integration.Lever.Options;

public sealed class LeverOptions
{
    public const string SectionName = "Lever";

    public string BaseUrl { get; set; } = "https://jobs.lever.co";

    public string ApiPath { get; set; } = "/api/v1";

    public int DefaultPageSize { get; set; } = 20;
}
