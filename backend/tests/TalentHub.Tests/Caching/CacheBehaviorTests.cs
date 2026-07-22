using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Communication.Options;
using TalentHub.Integration.Communication.Services;
using TalentHub.Integration.Lever.Models;
using TalentHub.Integration.Lever.Services;
using TalentHub.Integration.RemoteOK.Client;
using TalentHub.Integration.RemoteOK.Models;
using TalentHub.Integration.RemoteOK.Services;
using TalentHub.Integration.Greenhouse.Services;

namespace TalentHub.Tests.Caching;

public sealed class CacheBehaviorTests
{
    [Fact]
    public async Task RemoteOkService_caches_jobs_search_and_company_results()
    {
        var client = new RecordingRemoteOkClient();
        var service = new RemoteOkService(client, CreateMemoryCacheStore(), Options.Create(CreateCacheOptions()));

        await service.GetJobsAsync().ConfigureAwait(false);
        await service.GetJobsAsync().ConfigureAwait(false);
        await service.SearchJobsAsync(new RemoteOkSearchRequest("dotnet", "remote", 1, 20)).ConfigureAwait(false);
        await service.SearchJobsAsync(new RemoteOkSearchRequest("dotnet", "remote", 1, 20)).ConfigureAwait(false);
        await service.GetCompanyJobPreviewAsync("remoteok").ConfigureAwait(false);
        await service.GetCompanyJobPreviewAsync("remoteok").ConfigureAwait(false);

        Assert.Equal(1, client.GetJobsCalls);
        Assert.Equal(1, client.SearchCalls);
        Assert.Equal(1, client.CompanyCalls);
    }

    [Fact]
    public async Task CachedJobProvider_caches_provider_jobs_search_and_company_results()
    {
        var provider = new RecordingJobProvider();
        var cachedProvider = new CachedJobProvider(provider, CreateMemoryCacheStore(), Options.Create(CreateCacheOptions()));

        await cachedProvider.GetJobsAsync().ConfigureAwait(false);
        await cachedProvider.GetJobsAsync().ConfigureAwait(false);
        await cachedProvider.SearchJobsAsync(new ProviderJobQuery("dotnet", "remote", 1, 20)).ConfigureAwait(false);
        await cachedProvider.SearchJobsAsync(new ProviderJobQuery("dotnet", "remote", 1, 20)).ConfigureAwait(false);
        await cachedProvider.GetCompanyAsync("acme").ConfigureAwait(false);
        await cachedProvider.GetCompanyAsync("acme").ConfigureAwait(false);

        Assert.Equal(1, provider.GetJobsCalls);
        Assert.Equal(1, provider.SearchCalls);
        Assert.Equal(1, provider.CompanyCalls);
    }

    [Fact]
    public async Task JobAggregationService_caches_aggregated_results()
    {
        var provider = new RecordingJobProvider();
        var resolver = new DictionaryProviderResolver(new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["remoteok"] = provider
        });

        var service = new JobAggregationService(
            resolver,
            CreateMemoryCacheStore(),
            Options.Create(new ResilienceOptions()),
            Options.Create(CreateCacheOptions()));

        await service.GetJobsAsync().ConfigureAwait(false);
        await service.GetJobsAsync().ConfigureAwait(false);

        Assert.Equal(1, provider.GetJobsCalls);
    }

    [Fact]
    public async Task CacheInvalidator_invalidates_all_provider_namespaces()
    {
        var resolver = new DictionaryProviderResolver(new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["remoteok"] = new RecordingJobProvider(),
            ["lever"] = new RecordingJobProvider()
        });
        var cacheStore = CreateMemoryCacheStore();
        var invalidator = new CacheInvalidator(resolver, cacheStore);

        await cacheStore.SetAsync(CacheNamespaces.ForProviderJobs("remoteok"), "all", 1, new CacheEntryPolicy(TimeSpan.FromMinutes(5))).ConfigureAwait(false);
        await cacheStore.SetAsync(CacheNamespaces.ForProviderSearch("remoteok"), "DOTNET|REMOTE|1|20", 1, new CacheEntryPolicy(TimeSpan.FromMinutes(5))).ConfigureAwait(false);
        await cacheStore.SetAsync(CacheNamespaces.ForProviderCompany("lever"), "ACME", 1, new CacheEntryPolicy(TimeSpan.FromMinutes(5))).ConfigureAwait(false);
        await cacheStore.SetAsync(CacheNamespaces.AggregationJobs, "all", 1, new CacheEntryPolicy(TimeSpan.FromMinutes(5))).ConfigureAwait(false);

        await invalidator.InvalidateAllProviderCachesAsync().ConfigureAwait(false);
        await invalidator.InvalidateAggregationCacheAsync().ConfigureAwait(false);

        Assert.Null(await cacheStore.GetAsync<int?>(CacheNamespaces.ForProviderJobs("remoteok"), "all").ConfigureAwait(false));
        Assert.Null(await cacheStore.GetAsync<int?>(CacheNamespaces.ForProviderSearch("remoteok"), "DOTNET|REMOTE|1|20").ConfigureAwait(false));
        Assert.Null(await cacheStore.GetAsync<int?>(CacheNamespaces.ForProviderCompany("lever"), "ACME").ConfigureAwait(false));
        Assert.Null(await cacheStore.GetAsync<int?>(CacheNamespaces.AggregationJobs, "all").ConfigureAwait(false));
    }

    private static CacheOptions CreateCacheOptions()
        => new()
        {
            Enabled = true,
            ProviderJobsExpiration = TimeSpan.FromMinutes(5),
            ProviderSearchExpiration = TimeSpan.FromMinutes(2),
            ProviderCompanyExpiration = TimeSpan.FromMinutes(30),
            AggregationExpiration = TimeSpan.FromMinutes(10)
        };

    private static ICacheStore CreateMemoryCacheStore()
        => new MemoryCacheStore(new MemoryCache(new MemoryCacheOptions()), new CacheKeyBuilder());

    private sealed class RecordingRemoteOkClient : IRemoteOkClient
    {
        public int GetJobsCalls { get; private set; }

        public int SearchCalls { get; private set; }

        public int CompanyCalls { get; private set; }

        public Task<IReadOnlyList<RemoteOkJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            GetJobsCalls++;
            return Task.FromResult<IReadOnlyList<RemoteOkJobResponse>>(
                [
                    new RemoteOkJobResponse { Id = "1", Position = "Engineer", Company = "Acme", Location = "Remote", Remote = true, Date = DateTimeOffset.UtcNow }
                ]);
        }

        public Task<IReadOnlyList<RemoteOkJobResponse>> SearchJobsAsync(RemoteOkSearchRequest request, CancellationToken cancellationToken = default)
        {
            SearchCalls++;
            return Task.FromResult<IReadOnlyList<RemoteOkJobResponse>>(
                [
                    new RemoteOkJobResponse { Id = "search-1", Position = "Engineer", Company = "Acme", Location = "Remote", Remote = true, Date = DateTimeOffset.UtcNow }
                ]);
        }

        public Task<RemoteOkCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        {
            CompanyCalls++;
            return Task.FromResult<RemoteOkCompanyResponse?>(new RemoteOkCompanyResponse
            {
                Id = externalCompanyId,
                Name = "Acme",
                Website = "https://acme.example",
                Description = "Example company",
                Location = "Remote"
            });
        }
    }

    private sealed class RecordingJobProvider : IJobProvider
    {
        public int GetJobsCalls { get; private set; }

        public int SearchCalls { get; private set; }

        public int CompanyCalls { get; private set; }

        public string Name => "remoteok";

        public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            GetJobsCalls++;
            return Task.FromResult<IReadOnlyList<ProviderJob>>(
                [
                    new ProviderJob("1", "Engineer", "Acme", "Remote", "https://example.com/jobs/1", DateTimeOffset.UtcNow, true)
                ]);
        }

        public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
        {
            SearchCalls++;
            return Task.FromResult<IReadOnlyList<ProviderJob>>(
                [
                    new ProviderJob("search-1", "Engineer", "Acme", "Remote", "https://example.com/jobs/search-1", DateTimeOffset.UtcNow, true)
                ]);
        }

        public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        {
            CompanyCalls++;
            return Task.FromResult<ProviderCompany?>(new ProviderCompany(externalCompanyId, "Acme", "https://acme.example", "Example company", "Remote"));
        }
    }

    private sealed class DictionaryProviderResolver(IReadOnlyDictionary<string, IJobProvider> providers) : IProviderResolver
    {
        public IJobProvider Resolve(string providerName) => providers[providerName];

        public IReadOnlyCollection<string> GetProviderNames() => providers.Keys.ToArray();
    }
}
