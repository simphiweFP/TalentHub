using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Web.API.BackgroundWorkers;
using TalentHub.Web.API.Options;

namespace TalentHub.Tests.BackgroundWorkers;

public sealed class BackgroundWorkerTests
{
    [Fact]
    public async Task JobSynchronizationWorker_RunOnceAsync_calls_aggregation_service()
    {
        var aggregationService = new RecordingJobAggregationService(new JobAggregationResult
        {
            Jobs = [new Job { CompanyName = "Example", ExternalId = "1", PublishedAtUtc = DateTimeOffset.UtcNow, Source = "remoteok", Title = "Engineer" }],
            ProviderCount = 1,
            SuccessfulProviderCount = 1
        });

        var worker = new JobSynchronizationWorker(
            CreateScopeFactory(services => services.AddSingleton<IJobAggregationService>(aggregationService)),
            Options.Create(new BackgroundWorkerOptions()),
            NullLogger<JobSynchronizationWorker>.Instance);

        await worker.RunOnceAsync().ConfigureAwait(false);

        Assert.Equal(1, aggregationService.CallCount);
    }

    [Fact]
    public async Task CompanyRefreshWorker_RunOnceAsync_refreshes_unique_companies()
    {
        var provider = new RecordingJobProvider(new ProviderCompany("example", "Example Co", "https://example.com", "Description", "Remote"));
        var resolver = new RecordingProviderResolver(new Dictionary<string, IJobProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["remoteok"] = provider
        });
        var aggregationService = new RecordingJobAggregationService(new JobAggregationResult
        {
            Jobs =
            [
                new Job { CompanyName = "Example Co", ExternalId = "1", PublishedAtUtc = DateTimeOffset.UtcNow, Source = "remoteok", Title = "Engineer" },
                new Job { CompanyName = "Example Co", ExternalId = "2", PublishedAtUtc = DateTimeOffset.UtcNow, Source = "remoteok", Title = "Manager" }
            ]
        });

        var worker = new CompanyRefreshWorker(
            CreateScopeFactory(services =>
            {
                services.AddSingleton<IJobAggregationService>(aggregationService);
                services.AddSingleton<IProviderResolver>(resolver);
            }),
            Options.Create(new BackgroundWorkerOptions()),
            NullLogger<CompanyRefreshWorker>.Instance);

        await worker.RunOnceAsync().ConfigureAwait(false);

        Assert.Equal(["Example Co"], provider.RequestedCompanyIds);
        Assert.Equal(1, aggregationService.CallCount);
    }

    [Fact]
    public async Task ExpiredDataCleanupWorker_RunOnceAsync_executes_cleanup_commands()
    {
        var commandExecutor = new RecordingCommandExecutor();
        var worker = new ExpiredDataCleanupWorker(
            CreateScopeFactory(services => services.AddSingleton<ICommandExecutor>(commandExecutor)),
            Options.Create(new BackgroundWorkerOptions
            {
                SearchHistoryRetentionDays = 30,
                AuditLogRetentionDays = 90,
                ClosedJobRetentionDays = 45
            }),
            NullLogger<ExpiredDataCleanupWorker>.Instance);

        await worker.RunOnceAsync().ConfigureAwait(false);

        Assert.Equal(3, commandExecutor.Commands.Count);
        Assert.Contains(commandExecutor.Commands, command => command.Contains("SearchHistory", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(commandExecutor.Commands, command => command.Contains("AuditLogs", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(commandExecutor.Commands, command => command.Contains("Jobs", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task BackgroundWorkerScheduler_retries_and_stops_when_cancelled()
    {
        var attempts = 0;
        using var cancellationTokenSource = new CancellationTokenSource();

        await BackgroundWorkerScheduler.RunAsync(
            "TestWorker",
            TimeSpan.FromMilliseconds(1),
            token =>
            {
                attempts++;
                if (attempts == 1)
                {
                    throw new InvalidOperationException("boom");
                }

                cancellationTokenSource.Cancel();
                return Task.CompletedTask;
            },
            retryCount: 1,
            retryDelayMilliseconds: 0,
            NullLogger.Instance,
            cancellationTokenSource.Token).ConfigureAwait(false);

        Assert.Equal(2, attempts);
    }

    private static IServiceScopeFactory CreateScopeFactory(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);
        var provider = services.BuildServiceProvider();
        return new TestScopeFactory(provider);
    }

    private sealed class TestScopeFactory(IServiceProvider serviceProvider) : IServiceScopeFactory
    {
        public IServiceScope CreateScope() => new TestScope(serviceProvider.CreateScope());
    }

    private sealed class TestScope(IServiceScope scope) : IServiceScope
    {
        public IServiceProvider ServiceProvider => scope.ServiceProvider;

        public void Dispose() => scope.Dispose();
    }

    private sealed class RecordingJobAggregationService(JobAggregationResult result) : IJobAggregationService
    {
        public int CallCount { get; private set; }

        public Task<JobAggregationResult> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(result);
        }
    }

    private sealed class RecordingProviderResolver(IReadOnlyDictionary<string, IJobProvider> providers) : IProviderResolver
    {
        public IJobProvider Resolve(string providerName) => providers[providerName];

        public IReadOnlyCollection<string> GetProviderNames() => providers.Keys.ToArray();
    }

    private sealed class RecordingJobProvider(ProviderCompany company) : IJobProvider
    {
        public IReadOnlyList<string> RequestedCompanyIds { get; private set; } = [];

        public string Name => "remoteok";

        public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProviderJob>>([]);

        public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
            => GetJobsAsync(cancellationToken);

        public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        {
            RequestedCompanyIds = RequestedCompanyIds.Append(externalCompanyId).ToArray();
            return Task.FromResult<ProviderCompany?>(company);
        }
    }

    private sealed class RecordingCommandExecutor : ICommandExecutor
    {
        public List<string> Commands { get; } = [];

        public Task<int> ExecuteAsync(string sql, object? parameters = null, System.Data.IDbTransaction? transaction = null, int? commandTimeout = null, System.Data.CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            Commands.Add(sql);
            return Task.FromResult(1);
        }

        public Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, System.Data.IDbTransaction? transaction = null, int? commandTimeout = null, System.Data.CommandType? commandType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);
    }
}
