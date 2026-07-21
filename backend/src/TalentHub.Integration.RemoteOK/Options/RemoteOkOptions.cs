namespace TalentHub.Integration.RemoteOK.Options;

public sealed class RemoteOkOptions
{
    public const string SectionName = "RemoteOK";

    public string BaseUrl { get; set; } = "https://remoteok.com";

    public string ApiPath { get; set; } = "/api";

    public int DefaultPageSize { get; set; } = 20;
}
