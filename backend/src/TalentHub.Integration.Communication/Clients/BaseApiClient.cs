using System.Net.Http.Json;
using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Clients;

public abstract class BaseApiClient(ICommunicationHttpClientFactory clientFactory, IJsonSerializer serializer) : IBaseApiClient
{
    protected HttpClient Client => clientFactory.CreateClient(ClientName);

    protected virtual string ClientName => "communication";

    public async Task<TResponse?> GetAsync<TResponse>(string relativeUri, CancellationToken cancellationToken = default)
    {
        using var response = await Client.GetAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        return await ReadResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string relativeUri, TRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, relativeUri)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await Client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        return await ReadResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string relativeUri, TRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, relativeUri)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await Client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        return await ReadResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(string relativeUri, CancellationToken cancellationToken = default)
    {
        using var response = await Client.DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

    private async Task<TResponse?> ReadResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var payload = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"Request failed with status code {(int)response.StatusCode}: {payload}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(content) ? default : serializer.Deserialize<TResponse>(content);
    }
}
