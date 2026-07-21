namespace TalentHub.Integration.Communication.Options;

public sealed class CommunicationOptions
{
    public const string SectionName = "Communication";

    public string? DefaultBaseAddress { get; set; }

    public string CorrelationIdHeaderName { get; set; } = "X-Correlation-ID";

    public string AuthorizationHeaderName { get; set; } = "Authorization";
}
