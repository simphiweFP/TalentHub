using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Integration.Sql.Constants;
using TalentHub.Web.API.Options;

namespace TalentHub.Web.API.BackgroundWorkers;

public sealed class ExpiredDataCleanupWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<BackgroundWorkerOptions> options,
    ILogger<ExpiredDataCleanupWorker> logger) : BackgroundService
{
    public Task RunOnceAsync(CancellationToken cancellationToken = default)
        => ExecuteCycleAsync(cancellationToken);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => BackgroundWorkerScheduler.RunAsync(
            nameof(ExpiredDataCleanupWorker),
            options.Value.ExpiredDataCleanupInterval,
            ExecuteCycleAsync,
            options.Value.RetryCount,
            options.Value.RetryDelayMilliseconds,
            logger,
            stoppingToken);

    private async Task ExecuteCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var commandExecutor = scope.ServiceProvider.GetRequiredService<ICommandExecutor>();
        var cacheInvalidator = scope.ServiceProvider.GetService<TalentHub.Integration.Communication.Abstractions.ICacheInvalidator>();
        var now = DateTime.UtcNow;

        var searchHistoryCutoffUtc = now.AddDays(-Math.Max(1, options.Value.SearchHistoryRetentionDays));
        var auditLogCutoffUtc = now.AddDays(-Math.Max(1, options.Value.AuditLogRetentionDays));
        var closedJobCutoffUtc = now.AddDays(-Math.Max(1, options.Value.ClosedJobRetentionDays));

        var deletedSearchHistory = await commandExecutor.ExecuteAsync(
            $"DELETE FROM [dbo].[{TableNames.SearchHistory}] WHERE [SearchedAtUtc] < @SearchHistoryCutoffUtc",
            new { SearchHistoryCutoffUtc = searchHistoryCutoffUtc },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var deletedAuditLogs = await commandExecutor.ExecuteAsync(
            $"DELETE FROM [dbo].[{TableNames.AuditLogs}] WHERE [OccurredAtUtc] < @AuditLogCutoffUtc",
            new { AuditLogCutoffUtc = auditLogCutoffUtc },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var deletedJobs = await commandExecutor.ExecuteAsync(
            $"DELETE FROM [dbo].[{TableNames.Jobs}] WHERE [IsActive] = 0 AND [ClosedAtUtc] IS NOT NULL AND [ClosedAtUtc] < @ClosedJobCutoffUtc",
            new { ClosedJobCutoffUtc = closedJobCutoffUtc },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogInformation(
            "Cleaned expired data. SearchHistory={DeletedSearchHistory}, AuditLogs={DeletedAuditLogs}, Jobs={DeletedJobs}.",
            deletedSearchHistory,
            deletedAuditLogs,
            deletedJobs);

        if (cacheInvalidator is not null)
        {
            await cacheInvalidator.InvalidateAggregationCacheAsync(cancellationToken).ConfigureAwait(false);
            await cacheInvalidator.InvalidateProviderSearchCachesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
