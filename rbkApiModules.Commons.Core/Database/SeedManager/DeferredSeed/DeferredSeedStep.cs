using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational;

/// <summary>
/// Generic implementation of <see cref="IDeferredSeedStep"/> that runs a delegate against a specific DbContext type.
/// </summary>
public sealed class DeferredSeedStep<T> : IDeferredSeedStep where T : DbContext
{
    private readonly Action<T, IServiceProvider> _function;

    public DeferredSeedStep(string id, Action<T, IServiceProvider> function, EnvironmentUsage environmentUsage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(function);
        Id = id;
        _function = function;
        EnvironmentUsage = environmentUsage;
    }

    public string Id { get; }
    public EnvironmentUsage EnvironmentUsage { get; }
    public Type DbContextType => typeof(T);

    public void Execute(DbContext context, IServiceProvider serviceProvider)
    {
        _function((T)context, serviceProvider);
    }
}
