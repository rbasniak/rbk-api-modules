using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Core;

public abstract class TimedHostedService : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private Timer _timer;
    private Task _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
    private readonly IServiceProvider _services;

    public TimedHostedService(IServiceProvider services)
    {
        _services = services;
        _logger = _services.GetRequiredService<ILogger<TimedHostedService>>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ExecuteTask, null, FirstRunAfter, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }

    private void ExecuteTask(object state)
    {
        _timer?.Change(Timeout.Infinite, 0);

        _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
    }

    private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
    {
        try
        {
            using (var scope = _services.CreateScope())
            {
                await RunJobAsync(scope.ServiceProvider, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "BackgroundTask Failed");
        }

        _timer.Change(Interval, Timeout.InfiniteTimeSpan);
    }

    protected abstract Task RunJobAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken);

    protected abstract TimeSpan Interval { get; }

    protected abstract TimeSpan FirstRunAfter { get; }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        // Stop called without start
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            _stoppingCts.Cancel();
        }
        finally
        {
            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

    }

    public void Dispose()
    {
        _stoppingCts.Cancel();
        _timer?.Dispose();
    }
}