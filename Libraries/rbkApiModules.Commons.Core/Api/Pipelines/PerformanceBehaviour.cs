using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace rbkApiModules.Commons.Core.Pipelines;

/// <summary>
/// SCENARIO:
/// This behavior show how to put a central performance log point. This will log the 
/// username, the command payload and the elapsed time if greater than a threshold. 
/// Could also be used to log all requests not only slower ones
/// 
/// NOTES:
/// - This should be output in a dedicated log to easily pinpoint performance issues.
/// - We could also create a configuration on appsettings.json to dinamically enable
///   detailed logs of given commands. 
/// 
/// </summary>
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly Stopwatch _stopwatch;
    private readonly ILogger<TRequest> _logger;

    public PerformanceBehaviour(ILogger<TRequest> logger)
    {
        _stopwatch = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        _stopwatch.Start();

        var response = await next();

        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds > 1)
        {
            var requestName = typeof(TRequest).Name;
 
            _logger.LogWarning("Long running request: {requestName} ({ElapsedMilliseconds}ms) | {@Request}", requestName, _stopwatch.ElapsedMilliseconds, request);
        }

        return response;
    }
}