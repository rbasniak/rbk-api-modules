using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Relational.EventSourcing;

namespace rbkApiModules.Commons.Relational;

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

    public static DbContext GetEventStoreContext(this IEnumerable<DbContext> contexts)
    {
        var context = contexts.First();

        if (contexts.Count() > 1)
        {
            context = contexts.First(x => x is IEventStoreContext);
        }

        if (context == null) throw new NullReferenceException("Could not find an event store database context");

        return context;
    }
}
