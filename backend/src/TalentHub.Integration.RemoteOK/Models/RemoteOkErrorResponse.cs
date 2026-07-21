namespace TalentHub.Integration.RemoteOK.Models;

public sealed record RemoteOkErrorResponse
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
