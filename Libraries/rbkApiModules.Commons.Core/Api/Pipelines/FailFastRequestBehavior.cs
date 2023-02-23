using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace rbkApiModules.Commons.Core.Pipelines;

public class FailFastRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;
    private readonly IEnumerable<IValidator> _validators;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FailFastRequestBehavior(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor, ILogger<TRequest> logger)
    {
        _logger = logger;
        _validators = validators;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        // The base validator (the one for the specific command) comes from the pipeline
        var context = new ValidationContext<object>(request);

        // Then serarch for the other validators using the interfaces implemented by the command
        var interfaces = request.GetType().GetInterfaces().Where(x => !x.FullName.Contains("MediatR"));

        var composedValidators = _validators.DistinctBy(x => x.GetType()).ToList();

        foreach (var @interface in interfaces)
        {
            var ivalidator = typeof(IValidator<>);
            var generic = ivalidator.MakeGenericType(@interface);
            var validator = _httpContextAccessor.HttpContext.RequestServices.GetService(generic);

            if (validator != null)
            {
                composedValidators.Add((IValidator)validator);
            }
        }

        var failures = new List<ValidationFailure>();
            
        foreach (var composedValidator in composedValidators)
        {
            var validationResults = await composedValidator.ValidateAsync(context);

            foreach (var error in validationResults.Errors)
            {
                if (!String.IsNullOrEmpty(error.ErrorMessage) && error.ErrorMessage != "none")
                {
                    failures.Add(error);    
                }
            }
        }

        if (failures.Any())
        {
            return await Errors(failures);
        }
        else
        {
            return await next();
        } 
    }

    private static Task<TResponse> Errors(IEnumerable<ValidationFailure> failures)
    {
        var response = (TResponse)Activator.CreateInstance(typeof(TResponse));

        foreach (var failure in failures)
        {
            response.AddHandledError(failure.ErrorCode, failure.ErrorMessage);
        }

        return Task.FromResult(response as TResponse);
    }
}
