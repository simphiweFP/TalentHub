using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Web.API.Options;

namespace TalentHub.Web.API.BackgroundWorkers;

public sealed class JobSynchronizationWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<BackgroundWorkerOptions> options,
    ILogger<JobSynchronizationWorker> logger) : BackgroundService
{
    public Task RunOnceAsync(CancellationToken cancellationToken = default)
        => ExecuteCycleAsync(cancellationToken);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => BackgroundWorkerScheduler.RunAsync(
            nameof(JobSynchronizationWorker),
            options.Value.JobSyncInterval,
            ExecuteCycleAsync,
            options.Value.RetryCount,
            options.Value.RetryDelayMilliseconds,
            logger,
            stoppingToken);

    private async Task ExecuteCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var jobAggregationService = scope.ServiceProvider.GetRequiredService<IJobAggregationService>();
        var result = await jobAggregationService.GetJobsAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation(
            "Synchronized {JobCount} jobs from {ProviderCount} providers. Successful providers: {SuccessfulProviderCount}. Failed providers: {FailedProviderCount}.",
            result.Jobs.Count,
            result.ProviderCount,
            result.SuccessfulProviderCount,
            result.FailedProviderCount);

        foreach (var failure in result.Failures)
        {
            logger.LogWarning(
                "Provider {ProviderName} failed after {AttemptCount} attempts: {ErrorMessage}",
                failure.ProviderName,
                failure.AttemptCount,
                failure.ErrorMessage);
        }
    }
}
