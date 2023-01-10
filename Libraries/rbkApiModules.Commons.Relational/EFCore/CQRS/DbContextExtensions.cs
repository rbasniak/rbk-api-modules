using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational.CQRS;

public static class DbContextExtensions
{
    public static DbContext GetDefaultContext(this IEnumerable<DbContext> contexts)
    {
        var context = contexts.First();

        if (contexts.Count() > 1)
        {
            context = contexts.First(x => x is WriteDbContext || !(x is ReadDbContext));
        }

        if (context == null) throw new NullReferenceException("Could not find a default database context");

        return context;
    }
}
