namespace rbkApiModules.Commons.Core;

/// <summary>
/// Base interface for all requests. *** Not meant to be used directly by consumers. ***
/// </summary>
public interface IRequest<TResponse> { }


/// <summary>
/// Interface for untyped command type requests.
/// </summary>
public interface ICommand : IRequest<CommandResponse> { }

/// <summary>
/// Interface for typed command type requests.
/// </summary>
public interface ICommand<TResponse> : IRequest<CommandResponse<TResponse>> { }


/// <summary>
/// Interface for untyped query type requests.
/// </summary>
public interface IQuery : IRequest<QueryResponse> { }

/// <summary>
/// Interface for typed query type requests.
/// </summary>
public interface IQuery<TResponse> : IRequest<QueryResponse<TResponse>> { }


/// <summary>
/// Interface for untyped notification type requests.
/// </summary>
public interface INotification { }