using MediatR;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;

namespace rbkApiModules.Commons.Core.Pipelines;

/// <summary>
/// SCENARIO:
/// First level general exception handler, this handles exceptions between
/// the point in which the command is dispatched to MediatR until the point
/// it gets back to the endpoint after all pipeline is executed.
/// </summary>
public class ExceptionHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;
    private readonly ILocalizationService _localization;

    public ExceptionHandlerBehavior(ILogger<TRequest> logger, ILocalizationService localization)
    {
        _localization = localization;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        try
        {
            Log.Logger.Information("Exception handler pipeline started for {request}", request.GetType().FullName.Split('.').Last());

            var response = await next();

            Log.Logger.Information("Exception handler pipeline finished for {request}", request.GetType().FullName.Split('.').Last());

            return response;
        }
        catch (SafeException ex)
        {
            if (ex.ShouldBeLogged)
            {
                _logger.LogWarning(ex, "Exception thrown while handling MediatR command: {command}", request.GetType().FullName.Split('.').Last());
            }

            var response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[0]);

            response.AddHandledError(ex, ex.Message);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception thrown while handling MediatR command: {command}", request.GetType().FullName.Split('.').Last());

            var response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[0]);

            response.AddUnhandledError(ex, _localization.LocalizeString(SharedValidationMessages.Errors.InternalServerError));

            return response;
        }
    }
}