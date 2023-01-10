using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core.Auditing;

namespace rbkApiModules.Auditing.Relational;

public class RelationalTraceLogStore: ITraceLogStore
{
    private readonly DbContext _context;

    public RelationalTraceLogStore(IEnumerable<DbContext> contexts)
    {
        if (contexts.Count() > 1)
        {
            _context = contexts.FirstOrDefault(x => x.GetType().GetProperties().Any(x => x.PropertyType == typeof(DbSet<TraceLog>)));
        }
        else                                                                                                                                                                                     
        {
            _context = contexts.First();
        }
    }
    public async Task Add(params TraceLog[] data)
    {
        await _context.Set<TraceLog>().AddRangeAsync(data);
        await _context.SaveChangesAsync();
    }
}
