using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Represents a seed step that runs after application startup (e.g. when it depends on outbox/projection sync).
/// The consumer is responsible for waiting for whatever it needs before this step runs.
/// </summary>
public interface IDeferredSeedStep
{
    string Id { get; }
    EnvironmentUsage EnvironmentUsage { get; }
    Type DbContextType { get; }
    void Execute(DbContext context, IServiceProvider serviceProvider);
}
