namespace TalentHub.Integration.Lever.Models;

public sealed record LeverErrorResponse
{
    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
