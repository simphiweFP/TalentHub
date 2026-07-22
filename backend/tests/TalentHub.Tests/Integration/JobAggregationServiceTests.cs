using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Communication.Options;
using TalentHub.Integration.Communication.Services;

namespace TalentHub.Tests.Integration;

public sealed class JobAggregationServiceTests
{
    [Fact]
    public async Task GetJobsAsync_merges_normalizes_and_deduplicates_jobs()
    {
        var providers = new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["remoteok"] = new StubJobProvider(
                new[]
                {
                    new ProviderJob(" remoteok-1 ", " Senior .NET Engineer ", " RemoteOK ", " Remote ", "https://example.com/a", new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero), true),
                    new ProviderJob("remoteok-2", "Staff Engineer", "RemoteOK", "Remote", "https://example.com/b", new DateTimeOffset(2026, 7, 22, 10, 0, 0, TimeSpan.Zero), true)
                }),
            ["greenhouse"] = new StubJobProvider(
                new[]
                {
                    new ProviderJob("greenhouse-1", "Senior .NET Engineer", "RemoteOK", "Remote", "https://example.com/c", new DateTimeOffset(2026, 7, 22, 11, 0, 0, TimeSpan.Zero), true),
                    new ProviderJob("greenhouse-2", "Product Manager", "Greenhouse", "New York", "https://example.com/d", new DateTimeOffset(2026, 7, 22, 8, 0, 0, TimeSpan.Zero), false)
                })
        };

        var service = CreateService(providers);

        var result = await service.GetJobsAsync().ConfigureAwait(false);

        Assert.Equal(3, result.Jobs.Count);
        Assert.Equal(2, result.ProviderCount);
        Assert.Equal(2, result.SuccessfulProviderCount);
        Assert.Empty(result.Failures);
        Assert.Equal("Senior .NET Engineer", result.Jobs[0].Title);
        Assert.Equal("greenhouse", result.Jobs[0].Source);
        Assert.Equal("Staff Engineer", result.Jobs[1].Title);
        Assert.Equal("remoteok", result.Jobs[1].Source);
        Assert.Equal("Product Manager", result.Jobs[2].Title);
        Assert.Equal("RemoteOK", result.Jobs[0].CompanyName);
        Assert.Equal("Remote", result.Jobs[0].Location);
    }

    [Fact]
    public async Task GetJobsAsync_retries_before_succeeding()
    {
        var flakyProvider = new FlakyJobProvider(
            failuresBeforeSuccess: 2,
            new[]
            {
                new ProviderJob("lever-1", "Backend Engineer", "Lever", "Remote", "https://example.com/lever/1", DateTimeOffset.UtcNow, true)
            });

        var service = CreateService(new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["lever"] = flakyProvider
        }, retryCount: 2);

        var result = await service.GetJobsAsync().ConfigureAwait(false);

        Assert.Single(result.Jobs);
        Assert.Empty(result.Failures);
        Assert.Equal(3, flakyProvider.AttemptCount);
    }

    [Fact]
    public async Task GetJobsAsync_keeps_successful_providers_when_one_provider_fails()
    {
        var providers = new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["remoteok"] = new StubJobProvider(
                new[]
                {
                    new ProviderJob("remoteok-1", "Senior .NET Engineer", "RemoteOK", "Remote", "https://example.com/a", DateTimeOffset.UtcNow, true)
                }),
            ["greenhouse"] = new ThrowingJobProvider(new InvalidOperationException("provider unavailable"))
        };

        var service = CreateService(providers);

        var result = await service.GetJobsAsync().ConfigureAwait(false);

        Assert.Single(result.Jobs);
        Assert.Single(result.Failures);
        Assert.Equal("greenhouse", result.Failures[0].ProviderName);
        Assert.Equal("provider unavailable", result.Failures[0].ErrorMessage);
    }

    [Fact]
    public async Task GetJobsAsync_honors_cancellation_tokens()
    {
        var providers = new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["remoteok"] = new CancellationAwareJobProvider()
        };

        var service = CreateService(providers);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.GetJobsAsync(cancellationTokenSource.Token)).ConfigureAwait(false);
    }

    private static JobAggregationService CreateService(
        IReadOnlyDictionary<string, IJobProvider> providers,
        int retryCount = 0)
    {
        return new JobAggregationService(
            new DictionaryProviderResolver(providers),
            Options.Create(new ResilienceOptions
            {
                RetryCount = retryCount,
                RetryDelayMilliseconds = 0
            }));
    }

    private sealed class DictionaryProviderResolver(IReadOnlyDictionary<string, IJobProvider> providers) : IProviderResolver
    {
        public IJobProvider Resolve(string providerName) => providers[providerName];

        public IReadOnlyCollection<string> GetProviderNames() => providers.Keys.ToArray();
    }

    private sealed class StubJobProvider(IReadOnlyList<ProviderJob> jobs) : IJobProvider
    {
        public string Name => "stub";

        public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(jobs);

        public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
            => GetJobsAsync(cancellationToken);

        public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderCompany?>(null);
    }

    private sealed class FlakyJobProvider(int failuresBeforeSuccess, IReadOnlyList<ProviderJob> jobs) : IJobProvider
    {
        private int _attemptCount;

        public int AttemptCount => _attemptCount;

        public string Name => "flaky";

        public async Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            var attempt = Interlocked.Increment(ref _attemptCount);
            if (attempt <= failuresBeforeSuccess)
            {
                throw new InvalidOperationException($"attempt-{attempt}");
            }

            await Task.Yield();
            return jobs;
        }

        public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
            => GetJobsAsync(cancellationToken);

        public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderCompany?>(null);
    }

    private sealed class ThrowingJobProvider(Exception exception) : IJobProvider
    {
        public string Name => "throwing";

        public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
            => Task.FromException<IReadOnlyList<ProviderJob>>(exception);

        public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
            => GetJobsAsync(cancellationToken);

        public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderCompany?>(null);
    }

    private sealed class CancellationAwareJobProvider : IJobProvider
    {
        public string Name => "cancellable";

        public async Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
            return [];
        }

        public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
            => GetJobsAsync(cancellationToken);

        public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderCompany?>(null);
    }
}
