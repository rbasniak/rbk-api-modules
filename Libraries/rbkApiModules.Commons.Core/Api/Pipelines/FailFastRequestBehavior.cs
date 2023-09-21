using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace rbkApiModules.Commons.Core.Pipelines;

public class FailFastRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BaseResponse
{
    private readonly ILogger<TRequest> _logger;
    private readonly IEnumerable<IValidator> _validators;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static Type _domainValidatorType;
    private static bool _isDomainValidatorInitialized;

    public FailFastRequestBehavior(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor, ILogger<TRequest> logger)
    {
        _logger = logger;
        _validators = validators;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation)
    {
        Log.Logger.Information("Validation pipeline started for {request}", request.GetType().FullName.Split('.').Last());

        // The base validator (the one for the specific command) comes from the pipeline
        var context = new ValidationContext<object>(request);

        // Then serarch for the other validators using the interfaces implemented by the command
        var requestInterfaces = request.GetType().GetInterfaces().Where(x => !x.FullName.Contains("MediatR"));

        var composedValidators = _validators.ToList();

        foreach (var @interface in requestInterfaces)
        {
            var ivalidator = typeof(IValidator<>);
            var generic = ivalidator.MakeGenericType(@interface);
            var validator = _httpContextAccessor.HttpContext.RequestServices.GetService(generic);

            if (validator != null)
            {
                composedValidators.Add((IValidator)validator);
            }
        }


        if (!_isDomainValidatorInitialized)
        {
            _domainValidatorType = AppDomain.CurrentDomain.GetAssemblies()
               .Where(a => !a.IsDynamic)
               .SelectMany(a => a.GetTypes())
               .FirstOrDefault(t => t.FullName.Contains("RelationalDomainEntityValidator"));

            _isDomainValidatorInitialized = true;
        }

        var validatorsToAdd = new List<IValidator>();

        if (_domainValidatorType != null)
        {
            foreach (var validator in composedValidators)
            {

                var domainValidatorMarker = validator.GetType().GetInterfaces().Where(x => x.FullName.Contains(typeof(IDomainEntityValidator<>).Name)).FirstOrDefault();

                if (domainValidatorMarker == null) continue;

                var domainValidator = Activator.CreateInstance(_domainValidatorType, new object[] { validator });

                _logger.LogCritical("Could not instantiate the RelationalDomainValidator using reflection");

                if (domainValidator != null)
                {
                    validatorsToAdd.Add((IValidator)domainValidator);
                }
            }
        }

        composedValidators.InsertRange(0, validatorsToAdd);

        composedValidators = composedValidators.DistinctBy(x => x.GetType()).ToList();

        var failures = new List<ValidationFailure>();

        foreach (var composedValidator in composedValidators)
        {
            Log.Logger.Information("Running validator {validator}", composedValidator.GetType().FullName);

            var validationResults = await composedValidator.ValidateAsync(context);

            foreach (var error in validationResults.Errors)
            {
                if (!String.IsNullOrEmpty(error.ErrorMessage) && error.ErrorMessage != "none")
                {
                    Log.Logger.Information("Validator has error: {error}", error.ErrorMessage);

                    failures.Add(error);
                }
            }
        }

        if (failures.Any())
        {
            Log.Logger.Information("Validation pipeline finished with errors {request}", request.GetType().FullName.Split('.').Last());

            return await Errors(failures);
        }
        else
        {
            Log.Logger.Information("Validation pipeline finished for {request}", request.GetType().FullName.Split('.').Last());

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
