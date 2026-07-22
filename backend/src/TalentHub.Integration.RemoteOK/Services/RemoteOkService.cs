using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Options;
using TalentHub.Integration.Communication.Services;
using TalentHub.Integration.RemoteOK.Client;
using TalentHub.Integration.RemoteOK.Mapping;
using TalentHub.Integration.RemoteOK.Models;
using Job = TalentHub.Integration.Communication.Models.Job;

namespace TalentHub.Integration.RemoteOK.Services;

public sealed class RemoteOkService(
    IRemoteOkClient client,
    ICacheStore cacheStore,
    IOptions<CacheOptions> cacheOptions)
{
    public async Task<IReadOnlyList<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
        => await ExecuteCachedAsync(
            CacheNamespaces.ForProviderJobs("remoteok"),
            "all",
            cacheOptions.Value.ProviderJobsExpiration,
            async () => (await client.GetJobsAsync(cancellationToken).ConfigureAwait(false)).ToJobs(),
            cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<Job>> SearchJobsAsync(RemoteOkSearchRequest request, CancellationToken cancellationToken = default)
        => await ExecuteCachedAsync(
            CacheNamespaces.ForProviderSearch("remoteok"),
            BuildSearchKey(request),
            cacheOptions.Value.ProviderSearchExpiration,
            async () => (await client.SearchJobsAsync(request, cancellationToken).ConfigureAwait(false)).ToJobs(),
            cancellationToken).ConfigureAwait(false);

    public async Task<Job?> GetCompanyJobPreviewAsync(string externalCompanyId, CancellationToken cancellationToken = default)
    {
        var company = await ExecuteCachedAsync(
            CacheNamespaces.ForProviderCompany("remoteok"),
            externalCompanyId.Trim().ToUpperInvariant(),
            cacheOptions.Value.ProviderCompanyExpiration,
            () => client.GetCompanyAsync(externalCompanyId, cancellationToken),
            cancellationToken).ConfigureAwait(false);
        if (company is null)
        {
            return null;
        }

        return new Job
        {
            ExternalId = company.Id,
            Title = company.Name,
            CompanyName = company.Name,
            CompanyWebsite = company.Website,
            Location = company.Location,
            Description = company.Description,
            IsRemote = true,
            PublishedAtUtc = DateTimeOffset.UtcNow
        };
    }

    private Task<T> ExecuteCachedAsync<T>(string cacheNamespace, string cacheKey, TimeSpan expiration, Func<Task<T>> factory, CancellationToken cancellationToken)
    {
        if (!cacheOptions.Value.Enabled)
        {
            return factory();
        }

        return cacheStore.GetOrCreateAsync(cacheNamespace, cacheKey, new CacheEntryPolicy(expiration), _ => factory(), cancellationToken);
    }

    private static string BuildSearchKey(RemoteOkSearchRequest request)
        => string.Join('|',
            request.SearchTerm?.Trim().ToUpperInvariant() ?? string.Empty,
            request.Location?.Trim().ToUpperInvariant() ?? string.Empty,
            request.PageNumber,
            request.PageSize);
}
