using Microsoft.Extensions.Logging;

namespace TalentHub.Integration.Communication.Handlers;

public sealed class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var started = DateTimeOffset.UtcNow;
        logger.LogInformation("Sending HTTP {Method} {Uri}", request.Method, request.RequestUri);

        try
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Received HTTP {StatusCode} for {Method} {Uri} in {ElapsedMilliseconds}ms", (int)response.StatusCode, request.Method, request.RequestUri, (DateTimeOffset.UtcNow - started).TotalMilliseconds);
            return response;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "HTTP request failed for {Method} {Uri}", request.Method, request.RequestUri);
            throw;
        }
    }
}
