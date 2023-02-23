namespace rbkApiModules.Commons.Core.CQRS;

public class CqrsInMemoryStore<T> : ICqrsReadStore<T>
{
    private static IInMemoryDatabase<T> _context;

    public CqrsInMemoryStore(IInMemoryDatabase<T> context)
    {
        _context = context ?? throw new ArgumentNullException("Could not inject InMemoryDatabase, please check it is registered in the container");
    }

    public async Task AddAsync(T entity)
    {
        _context.Add(entity);

        await Task.CompletedTask;
    }


    public async Task<T> FindAsync(Guid id)
    {
        return await Task.FromResult(_context.FindAsync(id));
    }

    public async Task SaveChangesAsync()
    {
        await Task.CompletedTask;
    }

    public void Remove(Guid id, T entity)
    {
        _context.Remove(id, entity);
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        _context.UpdateAsync(entity);

        await Task.CompletedTask;
    }
}
