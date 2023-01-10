using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational;

public class SeedInfo<T> where T : DbContext
{
    public SeedInfo(Action<T> function, bool useInProduction)
    {
        Function = function;
        UseInProduction = useInProduction;
    }

    public Action<T> Function { get; }
    
    public bool UseInProduction { get; }
}