namespace TalentHub.Integration.Communication.Models;

public sealed record JobAggregationResult
{
    public IReadOnlyList<Job> Jobs { get; init; } = [];

    public IReadOnlyList<ProviderFailure> Failures { get; init; } = [];

    public int ProviderCount { get; init; }

    public int SuccessfulProviderCount { get; init; }

    public int FailedProviderCount => Failures.Count;
}
