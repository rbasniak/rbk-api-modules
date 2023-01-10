using MediatR;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        using (_logger.BeginScope(new Dictionary<string, object> 
        { 
            ["Command"] = typeof(TRequest).FullName,
        }))
        {
            return await next();

        }
    }
}