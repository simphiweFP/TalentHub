using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Handlers;

public sealed class CorrelationIdHandler(IOptions<CommunicationOptions> options, ICorrelationIdAccessor correlationIdAccessor) : DelegatingHandler
{
    private readonly CommunicationOptions _options = options.Value;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = correlationIdAccessor.CorrelationId ?? Guid.NewGuid().ToString("N");
        correlationIdAccessor.CorrelationId = correlationId;

        if (!request.Headers.Contains(_options.CorrelationIdHeaderName))
        {
            request.Headers.TryAddWithoutValidation(_options.CorrelationIdHeaderName, correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
