using System.Threading;

namespace TalentHub.Integration.Communication.Handlers;

public sealed class CircuitBreakerHandler : DelegatingHandler
{
    private readonly object _gate = new();
    private int _failureCount;
    private DateTimeOffset _openUntil = DateTimeOffset.MinValue;

    public int FailureThreshold { get; init; } = 5;

    public int BreakDurationSeconds { get; init; } = 30;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (_openUntil > DateTimeOffset.UtcNow)
            {
                throw new HttpRequestException("The circuit breaker is open.");
            }
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                lock (_gate)
                {
                    _failureCount = 0;
                    _openUntil = DateTimeOffset.MinValue;
                }
            }
            else
            {
                RegisterFailure();
            }

            return response;
        }
        catch
        {
            RegisterFailure();
            throw;
        }
    }

    private void RegisterFailure()
    {
        lock (_gate)
        {
            _failureCount++;
            if (_failureCount >= FailureThreshold)
            {
                _openUntil = DateTimeOffset.UtcNow.AddSeconds(BreakDurationSeconds);
                _failureCount = 0;
            }
        }
    }
}
