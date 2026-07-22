using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Services;

public sealed class CacheInvalidator(IProviderResolver providerResolver, ICacheStore cacheStore) : ICacheInvalidator
{
    public Task InvalidateAllProviderCachesAsync(CancellationToken cancellationToken = default)
        => Task.WhenAll(
            InvalidateProviderJobsCachesAsync(cancellationToken),
            InvalidateProviderSearchCachesAsync(cancellationToken),
            InvalidateProviderCompanyCachesAsync(cancellationToken));

    public async Task InvalidateProviderSearchCachesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var providerName in providerResolver.GetProviderNames())
        {
            await cacheStore.InvalidateNamespaceAsync(CacheNamespaces.ForProviderSearch(providerName), cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task InvalidateProviderCompanyCachesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var providerName in providerResolver.GetProviderNames())
        {
            await cacheStore.InvalidateNamespaceAsync(CacheNamespaces.ForProviderCompany(providerName), cancellationToken).ConfigureAwait(false);
        }
    }

    public Task InvalidateAggregationCacheAsync(CancellationToken cancellationToken = default)
        => cacheStore.InvalidateNamespaceAsync(CacheNamespaces.AggregationJobs, cancellationToken);

    private async Task InvalidateProviderJobsCachesAsync(CancellationToken cancellationToken)
    {
        foreach (var providerName in providerResolver.GetProviderNames())
        {
            await cacheStore.InvalidateNamespaceAsync(CacheNamespaces.ForProviderJobs(providerName), cancellationToken).ConfigureAwait(false);
        }
    }
}
