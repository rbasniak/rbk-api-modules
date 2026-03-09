using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Extension methods to register the deferred seed runner. Deferred steps run once after application startup;
/// the consumer is responsible for waiting for whatever it needs (e.g. projection sync) before its seed or handlers run.
/// </summary>
public static class DeferredSeedServiceCollectionExtensions
{
    /// <summary>
    /// Adds the deferred seed runner and configures optional deferred seed steps. Call <see cref="DeferredSeedRunnerOptions.AddDeferredSeed{T}"/>
    /// in the configure action to register steps that run after startup (e.g. seeds that depend on events/projections).
    /// </summary>
    public static IServiceCollection AddDeferredSeedRunner(this IServiceCollection services, Action<DeferredSeedRunnerOptions>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<DeferredSeedRunnerOptions>(_ => { });
        }

        services.AddSingleton<DeferredSeedRunnerHealth>();
        services.AddSingleton<IDeferredSeedRunnerHealth>(sp => sp.GetRequiredService<DeferredSeedRunnerHealth>());
        services.AddHostedService<DeferredSeedRunnerHostedService>();
        return services;
    }
}
