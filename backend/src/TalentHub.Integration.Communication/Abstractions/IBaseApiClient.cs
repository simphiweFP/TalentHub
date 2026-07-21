namespace TalentHub.Integration.Communication.Abstractions;

public interface IBaseApiClient
{
    Task<TResponse?> GetAsync<TResponse>(string relativeUri, CancellationToken cancellationToken = default);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string relativeUri, TRequest request, CancellationToken cancellationToken = default);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string relativeUri, TRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string relativeUri, CancellationToken cancellationToken = default);
}
