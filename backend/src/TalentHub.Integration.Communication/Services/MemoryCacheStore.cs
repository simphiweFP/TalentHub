using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Services;

public sealed class MemoryCacheStore(IMemoryCache memoryCache, ICacheKeyBuilder keyBuilder) : ICacheStore
{
    private readonly ConcurrentDictionary<string, long> _namespaceVersions = new(StringComparer.OrdinalIgnoreCase);

    public Task<T?> GetAsync<T>(string namespaceName, string key, CancellationToken cancellationToken = default)
        => Task.FromResult(memoryCache.TryGetValue(BuildVersionedKey(namespaceName, key), out T? value) ? value : default);

    public Task SetAsync<T>(string namespaceName, string key, T value, CacheEntryPolicy policy, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (policy.AbsoluteExpirationRelativeToNow > TimeSpan.Zero)
        {
            options.AbsoluteExpirationRelativeToNow = policy.AbsoluteExpirationRelativeToNow;
        }

        if (policy.SlidingExpiration is not null && policy.SlidingExpiration > TimeSpan.Zero)
        {
            options.SlidingExpiration = policy.SlidingExpiration;
        }

        memoryCache.Set(BuildVersionedKey(namespaceName, key), value, options);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrCreateAsync<T>(string namespaceName, string key, CacheEntryPolicy policy, Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken = default)
    {
        if (await GetAsync<T>(namespaceName, key, cancellationToken).ConfigureAwait(false) is { } existing)
        {
            return existing;
        }

        var created = await factory(cancellationToken).ConfigureAwait(false);
        await SetAsync(namespaceName, key, created, policy, cancellationToken).ConfigureAwait(false);
        return created;
    }

    public Task RemoveAsync(string namespaceName, string key, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(BuildVersionedKey(namespaceName, key));
        return Task.CompletedTask;
    }

    public Task InvalidateNamespaceAsync(string namespaceName, CancellationToken cancellationToken = default)
    {
        _namespaceVersions.AddOrUpdate(keyBuilder.BuildNamespace(namespaceName), 1, (_, current) => current + 1);
        return Task.CompletedTask;
    }

    private string BuildVersionedKey(string namespaceName, string key)
    {
        var normalizedNamespace = keyBuilder.BuildNamespace(namespaceName);
        var version = _namespaceVersions.GetOrAdd(normalizedNamespace, 1);
        return $"{normalizedNamespace}:{version}:{key}";
    }
}
