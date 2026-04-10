namespace rbkApiModules.Commons.Core;

/// <summary>
/// Interface for request handlers. *** Not meant to be used directly by consumers. ***
/// </summary>
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Interface for untyped command type request handlers.
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, CommandResponse>
    where TCommand : ICommand
{ }


/// <summary>
/// Interface for typed command type request handlers.
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, CommandResponse<TResponse>>
    where TCommand : ICommand<TResponse>
{ }


/// <summary>
/// Interface for untyped query type request handlers.
/// </summary>
public interface IQueryHandler<TQuery> : IRequestHandler<TQuery, QueryResponse> 
    where TQuery : IQuery
{ }

/// <summary>
/// Interface for typed query type request handlers.
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, QueryResponse<TResponse>>
    where TQuery : IQuery<TResponse>
{ }


public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
}