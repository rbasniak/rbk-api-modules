using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace rbkApiModules.Commons.Core;

public abstract class TimedBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;

    protected TimedBackgroundService(IServiceProvider services, ILoggerFactory loggerFactory)
    {
        _services = services;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    protected abstract TimeSpan Interval { get; }
    protected abstract TimeSpan FirstRunAfter { get; }

    /// <summary>
    /// Implement the unit of work. A fresh scope is created per execution.
    /// </summary>
    protected abstract Task RunJobAsync(IServiceProvider scopedProvider, CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (FirstRunAfter > TimeSpan.Zero)
        {
            try
            {
                await Task.Delay(FirstRunAfter, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
        }

        using var timer = new PeriodicTimer(Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                await RunJobAsync(scope.ServiceProvider, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background job failed");
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
                break;
            }
        }
    }
}