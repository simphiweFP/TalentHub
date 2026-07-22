using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Web.API.Options;

namespace TalentHub.Web.API.BackgroundWorkers;

public sealed class CompanyRefreshWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<BackgroundWorkerOptions> options,
    ILogger<CompanyRefreshWorker> logger) : BackgroundService
{
    public Task RunOnceAsync(CancellationToken cancellationToken = default)
        => ExecuteCycleAsync(cancellationToken);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => BackgroundWorkerScheduler.RunAsync(
            nameof(CompanyRefreshWorker),
            options.Value.CompanyRefreshInterval,
            ExecuteCycleAsync,
            options.Value.RetryCount,
            options.Value.RetryDelayMilliseconds,
            logger,
            stoppingToken);

    private async Task ExecuteCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var jobAggregationService = scope.ServiceProvider.GetRequiredService<IJobAggregationService>();
        var providerResolver = scope.ServiceProvider.GetRequiredService<IProviderResolver>();

        var result = await jobAggregationService.GetJobsAsync(cancellationToken).ConfigureAwait(false);
        var companyNames = result.Jobs
            .Select(job => job.CompanyName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var providerNames = providerResolver.GetProviderNames();
        var refreshTasks = providerNames.Select(providerName => RefreshProviderCompaniesAsync(providerName, companyNames, providerResolver, cancellationToken));
        await Task.WhenAll(refreshTasks).ConfigureAwait(false);

        logger.LogInformation(
            "Refreshed {CompanyCount} companies across {ProviderCount} providers.",
            companyNames.Length,
            providerNames.Count);
    }

    private async Task RefreshProviderCompaniesAsync(
        string providerName,
        IReadOnlyCollection<string> companyNames,
        IProviderResolver providerResolver,
        CancellationToken cancellationToken)
    {
        var provider = providerResolver.Resolve(providerName);

        foreach (var companyName in companyNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var company = await provider.GetCompanyAsync(companyName, cancellationToken).ConfigureAwait(false);
                if (company is null)
                {
                    continue;
                }

                logger.LogInformation(
                    "Refreshed company {CompanyName} from provider {ProviderName}.",
                    company.Name,
                    providerName);
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Failed to refresh company {CompanyName} from provider {ProviderName}.",
                    companyName,
                    providerName);
            }
        }
    }
}
