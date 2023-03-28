using rbkApiModules.Commons.Core.CQRS;
using LiteDB;
using rbkApiModules.Commons.Core.Utilities.Localization;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Commons.LiteDB;

public class CqrsLiteDBStore<T> : ICqrsReadStore<T> where T : class
{
    private readonly LiteDatabase _liteDatabase;
    private readonly ILocalizationService _localization;

    public CqrsLiteDBStore(ILocalizationService localization)
    {
        _liteDatabase = new LiteDatabase("");
        _localization = localization;
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
            throw new LiteException(-1, String.Format(_localization.LocalizeString(LiteDBCommonMessages.Errors.CannotDeleteEntity), id));
        }
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        var collection = _liteDatabase.GetCollection<T>(typeof(T).Name);

        var success = collection.Update(id, entity);

        if (!success)
        {
            throw new LiteException(-1, String.Format(_localization.LocalizeString(LiteDBCommonMessages.Errors.CannotUpdateEntity), id));
        }

        await Task.CompletedTask;
    }
}