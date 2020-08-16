using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure
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

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="validators">Validadoes (uso automático do MediatR)</param>
        public FailFastRequestBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Método que executa a validação em si (uso automático do MediatR)
        /// </summary>
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<object>(request);

            // Cuidado com Task.Result, pode ocasionar deadlocks.
            var failures = _validators
                .Select(async v => await v.ValidateAsync(context))
                .SelectMany(result => result.Result.Errors)
                .Where(f => f != null)
                .ToList();

            return failures.Any()
                ? Errors(failures)
                : next();
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