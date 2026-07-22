using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Services;

public sealed class DistributedCacheStore(IDistributedCache cache, ICacheKeyBuilder keyBuilder) : ICacheStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string namespaceName, string key, CancellationToken cancellationToken = default)
    {
        var payload = await cache.GetStringAsync(BuildVersionedKey(namespaceName, key), cancellationToken).ConfigureAwait(false);
        return payload is null ? default : JsonSerializer.Deserialize<T>(payload, JsonOptions);
    }

    public async Task SetAsync<T>(string namespaceName, string key, T value, CacheEntryPolicy policy, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (policy.AbsoluteExpirationRelativeToNow > TimeSpan.Zero)
        {
            options.AbsoluteExpirationRelativeToNow = policy.AbsoluteExpirationRelativeToNow;
        }

        if (policy.SlidingExpiration is not null && policy.SlidingExpiration > TimeSpan.Zero)
        {
            options.SlidingExpiration = policy.SlidingExpiration;
        }

        var payload = JsonSerializer.Serialize(value, JsonOptions);
        await cache.SetStringAsync(BuildVersionedKey(namespaceName, key), payload, options, cancellationToken).ConfigureAwait(false);
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
        => cache.RemoveAsync(BuildVersionedKey(namespaceName, key), cancellationToken);

    public Task InvalidateNamespaceAsync(string namespaceName, CancellationToken cancellationToken = default)
    {
        var versionKey = BuildVersionKey(namespaceName);
        return cache.SetStringAsync(versionKey, Guid.NewGuid().ToString("N"), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3650)
        }, cancellationToken);
    }

    private string BuildVersionedKey(string namespaceName, string key)
        => $"{keyBuilder.BuildNamespace(namespaceName)}:{GetVersion(namespaceName)}:{key}";

    private string BuildVersionKey(string namespaceName)
        => $"{keyBuilder.BuildNamespace(namespaceName)}:__version";

    private string GetVersion(string namespaceName)
        => cache.GetString(BuildVersionKey(namespaceName)) ?? "1";
}
