using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Options for the deferred seed runner. Use <see cref="AddDeferredSeed{T}"/> to register steps that run after startup.
/// </summary>
public class DeferredSeedRunnerOptions
{
    private readonly List<IDeferredSeedStep> _steps = new();

    /// <summary>
    /// Steps to run once after the application has started. The consumer is responsible for waiting as needed (e.g. for projection sync).
    /// </summary>
    public IReadOnlyList<IDeferredSeedStep> Steps => _steps;

    /// <summary>
    /// Registers a deferred seed step that runs after startup. Use for seeds that depend on events/projections (e.g. projects seed that needs materials in projection).
    /// </summary>
    public DeferredSeedRunnerOptions AddDeferredSeed<T>(string id, Action<T, IServiceProvider> function, EnvironmentUsage environmentUsage)
        where T : DbContext
    {
        _steps.Add(new DeferredSeedStep<T>(id, function, environmentUsage));
        return this;
    }
}
