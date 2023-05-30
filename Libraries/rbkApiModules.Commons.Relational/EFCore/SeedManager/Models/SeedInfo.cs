using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Commons.Relational;

public class SeedInfo<T> where T : DbContext
{
    public SeedInfo(Action<T, IServiceProvider> function, bool useInProduction)
    {
        Function = function;
        UseInProduction = useInProduction;
    }

    public Action<T, IServiceProvider> Function { get; }
    
    public bool UseInProduction { get; }
}