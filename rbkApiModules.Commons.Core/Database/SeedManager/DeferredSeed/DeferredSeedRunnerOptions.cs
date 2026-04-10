using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Options for the deferred seed runner. Use <see cref="AddDeferredSeed{T}"/> to register steps that run after startup.
/// </summary>
public class DeferredSeedRunnerOptions
{
    private readonly SortedList<string, IDeferredSeedStep> _steps = new();

    /// <summary>
    /// Steps to run once after the application has started. The consumer is responsible for waiting as needed (e.g. for projection sync).
    /// </summary>
    public IReadOnlyList<IDeferredSeedStep> Steps => _steps.Values.AsReadOnly();

    /// <summary>
    /// Registers a deferred seed step that runs after startup. Use for seeds that depend on events/projections (e.g. projects seed that needs materials in projection).
    /// </summary>
    public DeferredSeedRunnerOptions AddDeferredSeed<T>(IDeferredSeedStep seedStep)
        where T : DbContext
    {
        _steps.Add(seedStep.Id, seedStep);
        return this;
    }
}
