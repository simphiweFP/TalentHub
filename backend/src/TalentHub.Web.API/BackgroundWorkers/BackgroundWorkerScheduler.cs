using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TalentHub.Web.API.BackgroundWorkers;

internal static class BackgroundWorkerScheduler
{
    public static async Task RunAsync(
        string workerName,
        TimeSpan interval,
        Func<CancellationToken, Task> cycle,
        int retryCount,
        int retryDelayMilliseconds,
        ILogger logger,
        CancellationToken stoppingToken)
    {
        var effectiveInterval = interval <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : interval;

        using var timer = new PeriodicTimer(effectiveInterval);

        do
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await ExecuteWithRetryAsync(workerName, cycle, retryCount, retryDelayMilliseconds, logger, stoppingToken).ConfigureAwait(false);
                logger.LogInformation("{WorkerName} completed successfully in {ElapsedMilliseconds} ms.", workerName, stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{WorkerName} is shutting down.", workerName);
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{WorkerName} execution failed.", workerName);
            }

            try
            {
                if (!await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
                {
                    break;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{WorkerName} is shutting down.", workerName);
                break;
            }
        }
        while (true);
    }

    private static async Task ExecuteWithRetryAsync(
        string workerName,
        Func<CancellationToken, Task> cycle,
        int retryCount,
        int retryDelayMilliseconds,
        ILogger logger,
        CancellationToken stoppingToken)
    {
        var attempts = Math.Max(0, retryCount) + 1;
        var delayMilliseconds = Math.Max(0, retryDelayMilliseconds);
        Exception? lastException = null;

        for (var attempt = 1; attempt <= attempts; attempt++)
        {
            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                await cycle(stoppingToken).ConfigureAwait(false);
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception) when (attempt < attempts)
            {
                lastException = exception;
                logger.LogWarning(exception, "{WorkerName} attempt {Attempt} of {Attempts} failed; retrying.", workerName, attempt, attempts);

                if (delayMilliseconds > 0)
                {
                    await Task.Delay(delayMilliseconds * attempt, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                lastException = exception;
                break;
            }
        }

        throw new InvalidOperationException($"{workerName} failed after {attempts} attempts.", lastException);
    }
}
