using System.Collections.Concurrent;
using System.Data;

namespace rbkApiModules.Commons.Core;

public abstract class InMemoryDatabase<T> : IInMemoryDatabase<T> where T : BaseEntity
{
    private static bool _isInitialized = false;
    private static readonly ConcurrentDictionary<Guid, T> _database = new ConcurrentDictionary<Guid, T>();

    private readonly IServiceProvider _services;

    public InMemoryDatabase(IServiceProvider services) : base()
    {
        _services = services;
    }

    public IEnumerable<T> All() 
    {
        return _database.Select(x => x.Value);
    }

    public void Add(T entity) 
    {
        var success = _database.TryAdd(entity.Id, entity);

        if (!success)
        {
            throw new DBConcurrencyException("Could not add entity to the in memory database because it was already being tracked.");
        }
    }

    public T FindAsync(Guid id)
    {
        _database.TryGetValue(id, out var entity);

        return entity;
    } 

    public void Remove(Guid id, T entity) 
    {
        var success = _database.TryRemove(entity.Id, out var _);

        if (!success)
        {
            throw new DBConcurrencyException("Could not remove entity from the in memory database because it was not found.");
        }
    }

    public void UpdateAsync(T entity) 
    {
        var exists = _database.TryGetValue(entity.Id, out var oldEntity);

        if (!exists)
        {
            throw new DBConcurrencyException("Could not update entity from the in memory database because it was not found.");
        }

        var success = _database.TryUpdate(entity.Id, entity, oldEntity);

        if (!success)
        {
            throw new DBConcurrencyException("Could not update entity from the in memory database for unknown reasons.");
        }
    } 

    public void Initialize(Func<IServiceProvider, T[]> initialLoadFunction)
    {
        var entities = initialLoadFunction(_services);

        var _database = new ConcurrentDictionary<Guid, T>();

        foreach (var entity in entities)
        {
            _database.TryAdd(entity.Id, entity);
        }
    }
}