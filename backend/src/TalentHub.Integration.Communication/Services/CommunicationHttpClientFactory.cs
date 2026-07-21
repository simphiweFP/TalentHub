using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Services;

public sealed class CommunicationHttpClientFactory(IHttpClientFactory httpClientFactory) : ICommunicationHttpClientFactory
{
    public HttpClient CreateClient(string name) => httpClientFactory.CreateClient(name);
}
