namespace rbkApiModules.Commons.Core;

public interface IDatabaseTransactionHandler
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    void Dispose();
    void Rollback();
}
