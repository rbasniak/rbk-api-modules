namespace rbkApiModules.Commons.Core;

public interface IInMemoryDatabase
{
    IEnumerable<T> Set<T>() where T : BaseEntity;
    void Add<T>(T entity) where T : BaseEntity;
    T FindAsync<T>(Guid id) where T : BaseEntity;
    object FindAsync(Type type, Guid id);
    void Remove<T>(T entity) where T : BaseEntity;
    void UpdateAsync<T>(T entity) where T : BaseEntity;
    void Initialize(Type type, Func<IServiceProvider, BaseEntity[]> initialLoadFunction);
}