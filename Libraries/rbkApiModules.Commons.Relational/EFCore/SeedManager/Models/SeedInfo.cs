using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Commons.Relational;

public class SeedInfo<T> where T : DbContext
{
    public SeedInfo(Action<T, IServiceProvider> function, EnvironmentUsage environmentUsage)
    {
        Function = function;
        EnvironmentUsage = environmentUsage;
    }

    public Action<T, IServiceProvider> Function { get; }
    
    public EnvironmentUsage EnvironmentUsage { get; }
}

[Flags]
public enum EnvironmentUsage
{
    Production,
    Development,
    Testing
}