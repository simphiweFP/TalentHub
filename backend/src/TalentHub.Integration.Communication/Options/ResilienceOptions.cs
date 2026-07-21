namespace TalentHub.Integration.Communication.Options;

public sealed class ResilienceOptions
{
    public const string SectionName = "Communication:Resilience";

    public int RetryCount { get; set; } = 3;

    public int RetryDelayMilliseconds { get; set; } = 250;

    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    public int CircuitBreakerBreakDurationSeconds { get; set; } = 30;

    public int TimeoutSeconds { get; set; } = 100;
}
