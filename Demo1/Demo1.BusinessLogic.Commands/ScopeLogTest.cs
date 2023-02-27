using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Commands;

/// <summary>
/// 
/// SCENARIO:
/// This method demonstrates how scoped log could be implemented
/// application-context-id and screen-context-id could be sent
/// in the request header and then a middleware captures it and
/// creates a scope with those properties. We could even include
/// some extra information, like the username or other relevant
/// bits of information that are valid request wide.
/// 
/// NOTES:
///   - The handler calls Service4, which calls Service3, then 
///   Service2 and finally Service1. Each one has a Thread.Sleep
///   so their execution overlaps when they're called close to
///   each other. In the logs we can see that each context is
///   different for them. Allowing for easier debugging and 
///   log analysis of the informatin flow.
/// </summary>
public class ScopeLogTest
{
    public class Request: IRequest<CommandResponse> 
    {

    }

    public class Validator: AbstractValidator<Request>
    {
        
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IService4 _service4;

        public Handler(ILogger<Handler> logger, IService4 service4)
        {
            _logger = logger;
            _service4 = service4;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var scope1 = _logger.BeginScope(new Dictionary<string, object>
            {
                ["IsolatedPropertyHere"] = "This should be in this log entry only",
            });

            var scope2 = _logger.BeginScope(new Dictionary<string, object>
            {
                ["InnerIsolatedProperty"] = "This should not spill to other entries",
            });

            _logger.LogInformation($"Handling the request for ${typeof(Request).FullName}");

            scope1.Dispose();

            scope2.Dispose();

            _service4.Run();

            _logger.LogInformation($"Finished the request for ${typeof(Request).FullName}");

            return await Task.FromResult(CommandResponse.Success());
        }
    }
}