namespace TalentHub.Integration.Greenhouse.Models;

public sealed record GreenhouseErrorResponse
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
