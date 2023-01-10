using System.Collections.Concurrent;
using System.Data;

namespace rbkApiModules.Commons.Core;

public class InMemoryDatabase : IInMemoryDatabase
{
    private static bool _isInitialized = false;
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, BaseEntity>> _database = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, BaseEntity>>();

    private readonly IServiceProvider _services;

    public InMemoryDatabase(IServiceProvider services) : base()
    {
        _services = services;
    }

    public IEnumerable<T> Set<T>() where T : BaseEntity
    {
        var data = GetTable(typeof(T));

        return data.Select(x => (T)x.Value);
    }

    public void Add<T>(T entity) where T : BaseEntity
    {
        var data = GetTable(typeof(T));

        var success = data.TryAdd(entity.Id, entity);

        if (!success)
        {
            throw new DBConcurrencyException("Could not add entity to the in memory database because it was already being tracked.");
        }
    }

    public object FindAsync(Type type, Guid id)
    {
        var data = GetTable(type);

        data.TryGetValue(id, out var entity);

        return entity;
    }

    public T FindAsync<T>(Guid id) where T : BaseEntity
    {
        var data = GetTable(typeof(T));

        data.TryGetValue(id, out var entity);

        return (T)entity;
    }

    public void Remove<T>(T entity) where T : BaseEntity
    {
        var data = GetTable(typeof(T));

        var success = data.TryRemove(entity.Id, out var _);

        if (!success)
        {
            throw new DBConcurrencyException("Could not remove entity from the in memory database because it was not found.");
        }
    }

    public void UpdateAsync<T>(T entity) where T : BaseEntity
    {
        var data = GetTable(typeof(T));

        var exists = data.TryGetValue(entity.Id, out var oldEntity);

        if (!exists)
        {
            throw new DBConcurrencyException("Could not update entity from the in memory database because it was not found.");
        }

        var success = data.TryUpdate(entity.Id, entity, oldEntity);

        if (!success)
        {
            throw new DBConcurrencyException("Could not update entity from the in memory database for unknown reasons.");
        }
    }

    private ConcurrentDictionary<Guid, BaseEntity> GetTable(Type type)
    {
        var success = _database.TryGetValue(type, out var data);

        if (!success)
        {
            throw new KeyNotFoundException("Could not find the table in the in memory database. Please check that it's been properly setup and initialized");
        }

        return data;
    }

    public void Initialize(Type type, Func<IServiceProvider, BaseEntity[]> initialLoadFunction)
    {
        var entities = initialLoadFunction(_services);

        var tableData = new ConcurrentDictionary<Guid, BaseEntity>();

        foreach (var entity in entities)
        {
            tableData.TryAdd(entity.Id, entity);
        }

        _database.TryAdd(type, tableData);
    }
}