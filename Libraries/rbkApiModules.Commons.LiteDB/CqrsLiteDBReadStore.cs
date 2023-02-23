using rbkApiModules.Commons.Core.CQRS;
using LiteDB;

namespace rbkApiModules.Commons.LiteDB;

public class CqrsLiteDBStore<T> : ICqrsReadStore<T> where T : class
{
    private readonly LiteDatabase _liteDatabase;

    public CqrsLiteDBStore()
    {
        _liteDatabase = new LiteDatabase("");
    }

    public Task AddAsync(T entity)
    {
        var collection = _liteDatabase.GetCollection<T>(typeof(T).Name);

        collection.Insert((T)entity);

        _liteDatabase.Commit();

        return Task.CompletedTask;
    }


    public Task<T> FindAsync(Guid id)
    {
        var collection = _liteDatabase.GetCollection<T>(typeof(T).Name);

        var entity = collection.FindById(id);

        return Task.FromResult(entity);
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }

    public void Remove(Guid id, T entity)
    {
        var collection = _liteDatabase.GetCollection<T>(typeof(T).Name);

        var success = collection.Delete(id);

        if (!success)
        {
            throw new LiteException(-1, $"Could not delete the entity with id: {id}");
        }
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        var collection = _liteDatabase.GetCollection<T>(typeof(T).Name);

        var success = collection.Update(id, entity);

        if (!success)
        {
            throw new LiteException(-1, $"Could not update entity with id: {id}");
        }

        await Task.CompletedTask;
    }
}