namespace TalentHub.Integration.Communication.Models;

public sealed record SalaryInsightResult
{
    public string Summary { get; init; } = string.Empty;

    public string? CurrencyCode { get; init; }

    public decimal? EstimatedMinimum { get; init; }

    public decimal? EstimatedMaximum { get; init; }

    public IReadOnlyList<string> Factors { get; init; } = [];
}
