namespace rbkApiModules.Commons.Core.CQRS;

public interface ICqrsReadStore<T>
{
    Task<T> FindAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(Guid id, T entity);
    void Remove(Guid id, T entity);
    Task SaveChangesAsync();
}

