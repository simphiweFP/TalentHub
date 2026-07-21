namespace TalentHub.Integration.Communication.Handlers;

public sealed class TimeoutHandler : DelegatingHandler
{
    public int TimeoutSeconds { get; init; } = 100;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(TimeoutSeconds));

        try
        {
            return await base.SendAsync(request, timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The request timed out after {TimeoutSeconds} seconds.");
        }
    }
}
