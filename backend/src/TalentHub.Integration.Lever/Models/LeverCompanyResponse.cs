namespace TalentHub.Integration.Lever.Models;

public sealed record LeverCompanyResponse
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Website { get; init; }

    public string? Description { get; init; }

    public string? Location { get; init; }
}
