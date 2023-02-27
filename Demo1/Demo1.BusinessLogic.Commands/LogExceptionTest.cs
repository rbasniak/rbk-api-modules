using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Commands;

/// <summary>
/// 
/// SCENARIO:
/// This method demonstrates how exceptions are being logged
/// with Serilog. They have much more details out of the box
/// than with NLog. It gets all inner exceptions by default
/// with their extra data
/// 
/// NOTES:
/// - Handled exceptions are normally logged and treated, but
///   unhandled exceptions have two central points of treatment
///   one in the base handler in the MediatR pipeline and other
///   in a general middleware
/// </summary>
public class LogExceptionTest
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

        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var lowestException = new ApplicationException("Oh man, so many exceptions, but this should be interesting");

                lowestException.Data.Add("Custom Data", "Custom Value");
                lowestException.Data.Add("Operating System", Environment.OSVersion);

                throw new NotImplementedException("The method has not been implemented yet",
                    new InvalidDataException("The data is not valid",
                        lowestException));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Test with lots of inner exceptions");
            }

            return await Task.FromResult(CommandResponse.Success());
        }
    }
}