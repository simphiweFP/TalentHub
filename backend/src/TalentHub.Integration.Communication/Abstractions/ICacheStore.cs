using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Abstractions;

public interface ICacheStore
{
    Task<T?> GetAsync<T>(string namespaceName, string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string namespaceName, string key, T value, CacheEntryPolicy policy, CancellationToken cancellationToken = default);

    Task<T> GetOrCreateAsync<T>(string namespaceName, string key, CacheEntryPolicy policy, Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken = default);

    Task RemoveAsync(string namespaceName, string key, CancellationToken cancellationToken = default);

    Task InvalidateNamespaceAsync(string namespaceName, CancellationToken cancellationToken = default);
}
