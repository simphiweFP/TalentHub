using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Services;

public sealed class CachedJobProvider(
    IJobProvider inner,
    ICacheStore cacheStore,
    IOptions<CacheOptions> cacheOptions) : IJobProvider
{
    private readonly CacheOptions _options = cacheOptions.Value;

    public string Name => inner.Name;

    public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        => ExecuteCachedAsync(
            CacheNamespaces.ForProviderJobs(Name),
            "all",
            _options.ProviderJobsExpiration,
            () => inner.GetJobsAsync(cancellationToken),
            cancellationToken);

    public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
        => ExecuteCachedAsync(
            CacheNamespaces.ForProviderSearch(Name),
            BuildSearchKey(query),
            _options.ProviderSearchExpiration,
            () => inner.SearchJobsAsync(query, cancellationToken),
            cancellationToken);

    public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        => ExecuteCachedAsync(
            CacheNamespaces.ForProviderCompany(Name),
            Normalize(externalCompanyId),
            _options.ProviderCompanyExpiration,
            () => inner.GetCompanyAsync(externalCompanyId, cancellationToken),
            cancellationToken);

    private Task<T> ExecuteCachedAsync<T>(string cacheNamespace, string cacheKey, TimeSpan expiration, Func<Task<T>> factory, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return factory();
        }

        return cacheStore.GetOrCreateAsync(
            cacheNamespace,
            cacheKey,
            new CacheEntryPolicy(expiration),
            _ => factory(),
            cancellationToken);
    }

    private static string BuildSearchKey(ProviderJobQuery query)
        => string.Join('|',
            Normalize(query.SearchTerm),
            Normalize(query.Location),
            query.PageNumber,
            query.PageSize);

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToUpperInvariant();
}