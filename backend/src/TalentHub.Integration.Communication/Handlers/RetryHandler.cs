namespace TalentHub.Integration.Communication.Handlers;

public sealed class RetryHandler : DelegatingHandler
{
    public int RetryCount { get; init; } = 3;

    public int RetryDelayMilliseconds { get; init; } = 250;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;
        Exception? lastException = null;

        for (var attempt = 0; attempt <= RetryCount; attempt++)
        {
            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!ShouldRetry(response.StatusCode) || attempt == RetryCount)
                {
                    return response;
                }
            }
            catch (Exception exception) when (attempt < RetryCount)
            {
                lastException = exception;
            }

            if (attempt < RetryCount)
            {
                await Task.Delay(RetryDelayMilliseconds * (attempt + 1), cancellationToken).ConfigureAwait(false);
            }
        }

        if (lastException is not null)
        {
            throw lastException;
        }

        return response ?? throw new HttpRequestException("Retry handler did not receive a response.");
    }

    private static bool ShouldRetry(System.Net.HttpStatusCode statusCode)
        => statusCode is System.Net.HttpStatusCode.RequestTimeout or System.Net.HttpStatusCode.TooManyRequests or >= System.Net.HttpStatusCode.InternalServerError;
}
