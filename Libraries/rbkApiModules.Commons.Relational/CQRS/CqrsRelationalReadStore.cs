using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core.CQRS;

namespace rbkApiModules.Commons.Relational.CQRS;

public abstract class CqrsRelationalStore<T> : ICqrsReadStore<T>
{
    private readonly IEnumerable<DbContext> _contexts;

    public CqrsRelationalStore(IEnumerable<DbContext> contexts)
    {
        _contexts = contexts;
    }

    public async Task AddAsync(T entity)
    {
        var context = GetReadContext();

        await context.AddAsync(entity);
    }


    public async Task<T> FindAsync(Guid id)
    {
        var context = GetReadContext();

        var result = await context.FindAsync(typeof(T), id);

        return (T)result;
    }

    public async Task SaveChangesAsync()
    {
        var context = GetReadContext();

        await context.SaveChangesAsync();
    }

    public void Remove(Guid id, T entity)
    {
        var context = GetReadContext();

        context.Remove(entity);
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        var context = GetReadContext();

        context.Update(entity);

        await Task.CompletedTask;
    }

    private DbContext GetReadContext()
    {
        var filteredContext = _contexts.Where(context => typeof(ReadDbContext).IsAssignableFrom(context.GetType())).FirstOrDefault();

        if (filteredContext == null)
        {
            throw new NotImplementedException("To use the CQRS behavior pipeline you must have a DbContext inheriting from ReadDbContext");
        }

        return filteredContext;
    }
}