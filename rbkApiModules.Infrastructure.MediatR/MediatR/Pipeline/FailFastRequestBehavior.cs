using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    /// <summary>
    /// Behavior do MediatR para validação automática dos comandos
    /// </summary>
    /// <typeparam name="TRequest">Tipo do request (uso automático do MediatR)</typeparam>
    /// <typeparam name="TResponse">Tipo da resposta (uso automático do MediatR)</typeparam>
    public class FailFastRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<BaseResponse>
        where TResponse : BaseResponse
    {
        private readonly IEnumerable<IValidator> _validators;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="validators">Validadoes (uso automático do MediatR)</param>
        public FailFastRequestBehavior(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor)
        {
            _validators = validators;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Método que executa a validação em si (uso automático do MediatR)
        /// </summary>
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                // The base validator (the one for the specific command) comes from the pipeline
                var context = new ValidationContext<object>(request);

                // Then serarch for the other validators using the interfaces implemented by the command
                var interfaces = request.GetType().GetInterfaces().Where(x => !x.FullName.Contains("MediatR"));

                var composedValidators = _validators.ToList();

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

                // Cuidado com Task.Result, pode ocasionar deadlocks.
                var failures = composedValidators
                    .Select(async v => await v.ValidateAsync(context))
                    .SelectMany(result => result.Result.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Any())
                {
                    return Errors(failures);
                }
                else
                {
                    // Finally, use the database common validations if the validation attributes are used in the command
                    var dbValidatorService = _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ICommonDatabaseValidations)) as ICommonDatabaseValidations;

                    if (dbValidatorService != null)
                    {
                        var result = dbValidatorService.ValidateExistingDbElements(_httpContextAccessor, request);

                        if (result.Length > 0)
                        {
                            return Errors(result);
                        }
                    }

                    // No errors so far, proceed to the next item in the pipeline
                    return next();
                }
            }
            catch (SafeException ex)
            {
                return Errors(new List<ValidationFailure> { new ValidationFailure(null, ex.Message) });
            } 
            catch (Exception ex)
            {
                var diagnosticsStore = _httpContextAccessor.HttpContext.RequestServices.GetService<IDiagnosticsModuleStore>();

                if (diagnosticsStore != null)
                {
                    var exceptionData = new DiagnosticsEntry(_httpContextAccessor.HttpContext, request.GetType().FullName + " (Validator)", ex, request);
                    diagnosticsStore.StoreData(exceptionData);
                }

                return Errors(new List<ValidationFailure> { new ValidationFailure(null, "Erro interno no servidor durante a validação dos dados") });
            }
        }

        private Task<TResponse> Errors(Models.ValidationResult[] results)
        {
            var response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[0]);

            response.AddHandledError(new ModelValidationException(results));

            return Task.FromResult(response as TResponse);
        }

        /// <summary>
        /// Adiciona erros de validação no retorno do comando e seta o status para inválido
        /// </summary>
        /// <param name="failures">Erros de validação</param>
        /// <returns></returns>
        private static Task<TResponse> Errors(IEnumerable<ValidationFailure> failures)
        {
            var response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[0]);

            foreach (var failure in failures)
            {
                response.AddHandledError(failure.ErrorMessage);
            }

            return Task.FromResult(response as TResponse);
        }
    }
} 