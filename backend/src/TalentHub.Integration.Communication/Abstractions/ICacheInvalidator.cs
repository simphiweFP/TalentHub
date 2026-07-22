namespace TalentHub.Integration.Communication.Abstractions;

public interface ICacheInvalidator
{
    Task InvalidateAllProviderCachesAsync(CancellationToken cancellationToken = default);

    Task InvalidateProviderSearchCachesAsync(CancellationToken cancellationToken = default);

    Task InvalidateProviderCompanyCachesAsync(CancellationToken cancellationToken = default);

    Task InvalidateAggregationCacheAsync(CancellationToken cancellationToken = default);
}
