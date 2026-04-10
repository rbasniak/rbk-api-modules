using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational;

public static class DbContextExtensions
{
    public static DbContext GetDefaultContext(this IEnumerable<DbContext> contexts)
    {
        var context = contexts.First();

        // TODO: Still need to handle multiple contexts with the simpler version of the library?
        // if (contexts.Count() > 1)
        // {
        //     context = contexts.First(x => x is WriteDbContext || !(x is ReadDbContext));
        // }

        // if (context == null) throw new NullReferenceException("Could not find a default database context");

        return context;
    }
}