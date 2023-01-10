using MediatR;
using Microsoft.Extensions.Logging;

namespace rbkApiModules.Commons.Core.Pipelines;

/// <summary>
/// SCENARIO:
/// To avoid having the user starting a new transaction every time, each command processed is wrapped
/// in it's own transaction. When the command fails, the transaction is automatically rolled back.
/// 
/// NOTES:
/// Doing this for all commands might not be desirable depending on the application requirements. 
/// But it's easy controllable by creating an interface and implementing it into the desirable commands.
/// 
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly IDatabaseTransactionHandler _transactionHandler;
    private readonly ILogger<TRequest> _logger;

    public TransactionBehavior(ILogger<TRequest> logger, IDatabaseTransactionHandler transactionHandler)
    {
        _transactionHandler = transactionHandler;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        if (typeof(IRequest<CommandResponse>).IsAssignableFrom(typeof(TRequest)) ||
            typeof(IRequest<AuditableCommandResponse>).IsAssignableFrom(typeof(TRequest)))
        {

            await _transactionHandler.BeginTransactionAsync();

            try
            {
                var response = await next();

                await _transactionHandler.CommitAsync();

                return response;
            }
            catch
            {
                _transactionHandler.Rollback();

                // Rethrow the exception to be handled by the previous behavior in the pipeline
                throw;
            }
            finally
            {
                _transactionHandler.Dispose();
            }
        }
        else
        {
            return await next();
        }
    }
}