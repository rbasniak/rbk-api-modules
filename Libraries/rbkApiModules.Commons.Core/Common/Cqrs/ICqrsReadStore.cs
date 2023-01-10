namespace rbkApiModules.Commons.Core.CQRS;

public interface ICqrsReadStore
{
    Task<object> FindAsync(Type type, Guid id);
    Task AddAsync(object entity);
    Task UpdateAsync(object entity);
    void Remove(object entity);
    Task SaveChangesAsync();
}

