using MediatR;
using Serilog;

namespace rbkApiModules.Commons.Core.Pipelines;

/// <summary>
/// SCENARIO:
/// This behavior show how to put a central log point to add a property
/// to all logs informing which command is being handled. Could be used
/// to add other scoped properties as well, but no much fuzz about it.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        _logger.ForContext("Command", typeof(TRequest).FullName);
        
        return await next();
    }
}