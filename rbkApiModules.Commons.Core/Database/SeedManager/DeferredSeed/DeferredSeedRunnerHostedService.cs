using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core.Helpers;

namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Runs deferred seed steps once after application startup. Does not wait for outboxes;
/// the consumer is responsible for waiting for whatever it needs before its seed or handlers run.
/// </summary>
public sealed class DeferredSeedRunnerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<DeferredSeedRunnerOptions> _options;
    private readonly IWebHostEnvironment _environment;
    private readonly DeferredSeedRunnerHealth _health;
    private readonly ILogger<DeferredSeedRunnerHostedService> _logger;

    public DeferredSeedRunnerHostedService(
        IServiceScopeFactory scopeFactory,
        IOptions<DeferredSeedRunnerOptions> options,
        IWebHostEnvironment environment,
        DeferredSeedRunnerHealth health,
        ILogger<DeferredSeedRunnerHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _environment = environment;
        _health = health;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var steps = _options.Value.Steps;
        if (steps.Count == 0)
        {
            _logger.LogDebug("No deferred seed steps registered.");
            _health.RecordCompleted();
            return;
        }

        _health.RecordRunning();
        _logger.LogInformation("Deferred seed runner started, {Count} step(s) registered.", steps.Count);

        using var scope = _scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var isProduction = _environment.IsProduction();
        var isDevelopment = !isProduction && !TestingEnvironmentChecker.IsTestingEnvironment;
        var isTesting = TestingEnvironmentChecker.IsTestingEnvironment;

        try
        {
            foreach (var step in steps)
            {
                var useInProduction = (step.EnvironmentUsage & EnvironmentUsage.Production) != 0;
                var useInDevelopment = (step.EnvironmentUsage & EnvironmentUsage.Development) != 0;
                var useInTest = (step.EnvironmentUsage & EnvironmentUsage.Testing) != 0;

                var shouldRun = useInDevelopment && isDevelopment
                    || useInProduction && isProduction
                    || useInTest && isTesting;

                if (!shouldRun)
                {
                    _logger.LogDebug("Skipping deferred seed step {StepId} (environment not applicable).", step.Id);
                    continue;
                }

                var context = (DbContext?)serviceProvider.GetService(step.DbContextType);
                if (context is null)
                {
                    _logger.LogWarning("DbContext {DbContextType} not registered; skipping deferred seed step {StepId}.",
                        step.DbContextType.Name, step.Id);
                    continue;
                }

                var alreadyRun = await context.Set<SeedHistory>().AnyAsync(x => x.Id == step.Id, stoppingToken);
                if (alreadyRun)
                {
                    _logger.LogDebug("Deferred seed step {StepId} already applied.", step.Id);
                    continue;
                }

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        await using var transaction = await context.Database.BeginTransactionAsync(stoppingToken);
                        try
                        {
                            step.Execute(context, serviceProvider);
                            context.Add(new SeedHistory(step.Id, DateTime.UtcNow));
                            await context.SaveChangesAsync(stoppingToken);
                            await transaction.CommitAsync(stoppingToken);
                            _health.RecordStepCompleted(step.Id);
                            _logger.LogInformation("Deferred seed step {StepId} completed.", step.Id);
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync(stoppingToken);
                            throw new DatabaseSeedException(ex, step.Id);
                        }
                    });
                }
                catch (DatabaseSeedException ex)
                {
                    _health.RecordFailed(step.Id, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _health.RecordFailed(step.Id, ex.Message);
                    throw new DatabaseSeedException(ex, step.Id);
                }
            }

            _health.RecordCompleted();
            _logger.LogInformation("Deferred seed runner finished.");
        }
        catch (DatabaseSeedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _health.RecordFailed(null, ex.Message);
            throw;
        }
    }
}
