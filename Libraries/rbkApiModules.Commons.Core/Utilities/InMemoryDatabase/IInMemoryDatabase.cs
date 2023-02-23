namespace rbkApiModules.Commons.Core;

public interface IInMemoryDatabase<T>
{
    IEnumerable<T> All();
    void Add(T entity);
    T FindAsync(Guid id);
    void Remove(Guid id, T entity);
    void UpdateAsync(T entity);
    void Initialize(Func<IServiceProvider, T[]> initialLoadFunction);
}