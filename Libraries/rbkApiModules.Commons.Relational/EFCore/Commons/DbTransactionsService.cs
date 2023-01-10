using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Commons.Relational;

public class DatabaseTransactionHandler : IDatabaseTransactionHandler
{
    private readonly DbContext _context;
    private IDbContextTransaction _transaction;

    public DatabaseTransactionHandler(DbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("You need to begin a transaction before commiting");
        }

        await _transaction.CommitAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }

    public void Rollback()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("You need to begin a transaction before trying to rollback");
        }

        _transaction.Rollback();
    }
}
