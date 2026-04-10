using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Commons.Core;

/// <summary>
/// Pipeline behavior that wraps command execution in a database transaction.
/// The transaction is committed if the command succeeds, or rolled back if an exception occurs.
/// Uses Entity Framework Core's execution strategy to handle retries for transient failures.
/// </summary>
/// <remarks>
/// This behavior only applies to commands (ICommand or ICommand&lt;T&gt;). 
/// Queries (IQuery or IQuery&lt;T&gt;) are executed without transactions since they don't modify data.
/// 
/// To enable this behavior, call AddTransactionalPipelineBehavior() in your service registration:
/// <code>
/// services.AddMessaging()
///         .AddTransactionalPipelineBehavior();
/// </code>
/// </remarks>
public class TransactionalPipelineBehavior<TRequest, TResponse>(
    IEnumerable<DbContext> dbContexts,
    ILogger<TransactionalPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        if (!IsCommand(request))
        {
            logger.LogDebug("Skipping transaction for non-command request {RequestType}", typeof(TRequest).Name);
            return await next();
        }

        if (ShouldSkipTransaction(request))
        {
            logger.LogDebug("Skipping transaction for {RequestType} because it implements ISkipTransaction", typeof(TRequest).Name);
            return await next();
        }

        var context = dbContexts.GetDefaultContext();

        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                logger.LogDebug("Started transaction for {RequestType}", typeof(TRequest).Name);

                var response = await next();

                await transaction.CommitAsync(cancellationToken);

                logger.LogDebug("Committed transaction for {RequestType}", typeof(TRequest).Name);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing {RequestType}, rolling back transaction", typeof(TRequest).Name);

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        });
    }

    private static bool IsCommand(TRequest request)
    {
        var requestType = request.GetType();

        if (request is ICommand)
        {
            return true;
        }

        var interfaces = requestType.GetInterfaces();
        return interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }

    private static bool ShouldSkipTransaction(TRequest request)
    {
        return request is ISkipTransaction;
    }
}


public interface ISkipTransaction
{
    // Marker interface to indicate that a request should skip transaction behavior
}