namespace rbkApiModules.Commons.Core.CQRS;

public class CqrsInMemoryStore : ICqrsReadStore
{
    private static IInMemoryDatabase _context;

    public CqrsInMemoryStore(IInMemoryDatabase context)
    {
        _context = context ?? throw new ArgumentNullException("Could not inject InMemoryDatabase, please check it is registered in the container");
    }

    public async Task AddAsync(object entity)
    {
        _context.Add((BaseEntity)entity);

        await Task.CompletedTask;
    }


    public async Task<object> FindAsync(Type type, Guid id)
    {
        return await Task.FromResult(_context.FindAsync(type, id));
    }

    public async Task SaveChangesAsync()
    {
        await Task.CompletedTask;
    }

    public void Remove(object entity)
    {
        _context.Remove((BaseEntity)entity);
    }

    public async Task UpdateAsync(object entity)
    {
        _context.Add((BaseEntity)entity);

        await Task.CompletedTask;
    }
}
