using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Services;

public sealed class JobAggregationService(
    IProviderResolver providerResolver,
    IOptions<ResilienceOptions> resilienceOptions) : IJobAggregationService
{
    public async Task<JobAggregationResult> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        var providerNames = providerResolver.GetProviderNames();
        if (providerNames.Count == 0)
        {
            return new JobAggregationResult();
        }

        var providerTasks = providerNames
            .Select(providerName => AggregateProviderAsync(providerName, cancellationToken))
            .ToArray();

        var providerResults = await Task.WhenAll(providerTasks).ConfigureAwait(false);

        var failures = providerResults
            .Where(result => result.Failure is not null)
            .Select(result => result.Failure!)
            .ToArray();

        var jobs = providerResults
            .SelectMany(result => result.Jobs)
            .GroupBy(BuildDeduplicationKey)
            .Select(group => group
                .OrderByDescending(job => job.PublishedAtUtc)
                .ThenBy(job => job.Source, StringComparer.OrdinalIgnoreCase)
                .ThenBy(job => job.ExternalId, StringComparer.OrdinalIgnoreCase)
                .First())
            .OrderByDescending(job => job.PublishedAtUtc)
            .ThenBy(job => job.CompanyName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(job => job.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new JobAggregationResult
        {
            Jobs = jobs,
            Failures = failures,
            ProviderCount = providerNames.Count,
            SuccessfulProviderCount = providerResults.Count(result => result.Failure is null)
        };
    }

    private async Task<ProviderAggregationResult> AggregateProviderAsync(string providerName, CancellationToken cancellationToken)
    {
        try
        {
            var provider = providerResolver.Resolve(providerName);
            var providerJobs = await ExecuteWithRetryAsync(
                () => provider.GetJobsAsync(cancellationToken),
                providerName,
                cancellationToken).ConfigureAwait(false);

            var jobs = providerJobs.Select(job => NormalizeJob(providerName, job)).ToArray();
            return new ProviderAggregationResult(providerName, jobs, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            var errorMessage = exception.InnerException?.Message ?? exception.Message;
            return new ProviderAggregationResult(
                providerName,
                [],
                new ProviderFailure(providerName, errorMessage, resilienceOptions.Value.RetryCount + 1));
        }
    }

    private async Task<IReadOnlyList<ProviderJob>> ExecuteWithRetryAsync(
        Func<Task<IReadOnlyList<ProviderJob>>> action,
        string providerName,
        CancellationToken cancellationToken)
    {
        var retryCount = Math.Max(0, resilienceOptions.Value.RetryCount);
        var delayMilliseconds = Math.Max(0, resilienceOptions.Value.RetryDelayMilliseconds);
        Exception? lastException = null;

        for (var attempt = 0; attempt <= retryCount; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception) when (attempt < retryCount)
            {
                lastException = exception;

                if (delayMilliseconds > 0)
                {
                    var delay = delayMilliseconds * (attempt + 1);
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                lastException = exception;
                break;
            }
        }

        throw new InvalidOperationException(
            $"Provider '{providerName}' failed after {retryCount + 1} attempts.",
            lastException);
    }

    private static Job NormalizeJob(string providerName, ProviderJob providerJob)
        => new()
        {
            Source = NormalizeText(providerName),
            ExternalId = NormalizeText(providerJob.ExternalId),
            Title = NormalizeText(providerJob.Title),
            CompanyName = NormalizeText(providerJob.CompanyName),
            CompanyWebsite = null,
            Location = NormalizeOptionalText(providerJob.Location),
            Url = NormalizeOptionalText(providerJob.Url),
            Description = null,
            IsRemote = providerJob.IsRemote,
            PublishedAtUtc = providerJob.PublishedAtUtc
        };

    private static string BuildDeduplicationKey(Job job)
        => string.Join('|',
            NormalizeText(job.Title).ToUpperInvariant(),
            NormalizeText(job.CompanyName).ToUpperInvariant(),
            NormalizeOptionalText(job.Location).ToUpperInvariant(),
            job.IsRemote ? "REMOTE" : "ONSITE");

    private static string NormalizeText(string value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    private static string? NormalizeOptionalText(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : NormalizeText(value);

    private sealed record ProviderAggregationResult(
        string ProviderName,
        IReadOnlyList<Job> Jobs,
        ProviderFailure? Failure);
}
