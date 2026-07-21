namespace TalentHub.Integration.Greenhouse.Options;

public sealed class GreenhouseOptions
{
    public const string SectionName = "Greenhouse";

    public string BaseUrl { get; set; } = "https://boards.greenhouse.io";

    public string ApiPath { get; set; } = "/api/v1";

    public int DefaultPageSize { get; set; } = 20;
}
