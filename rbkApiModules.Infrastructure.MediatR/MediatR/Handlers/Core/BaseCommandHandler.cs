using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Abstract;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    /// <summary>
    /// Classe base que todos os handlers devem herdar. Faz o tratamento de exceções automaticamente
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando tratado pelo handler</typeparam>
    public abstract class BaseCommandHandler<TCommand> : BaseHandler<TCommand, CommandResponse>, IRequestHandler<TCommand, CommandResponse>
        where TCommand : IRequest<CommandResponse>
    {
        public BaseCommandHandler(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        /// <summary>
        /// Método principal do handler, que executa as ações necessárias para processar 
        /// o comando (uso automático do MediatR)
        /// </summary>
        public async Task<CommandResponse> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            try
            {
                var result = await ExecuteAsync(request);

                response.EventData.EntityId = null;
                response.Result = result;
            }
            catch (KindaSafeException ex)
            {
                response.AddHandledError(ex.Message);
                StoreDiagnosticsData(request, ex);
            }
            catch (SafeException ex)
            {
                response.AddHandledError(ex.Message);
            }
            catch (ModelValidationException ex)
            {
                response.AddHandledError(ex);
            }
            catch (Exception ex)
            {
                response.AddUnhandledError(ex.Message);
                StoreDiagnosticsData(request, ex);
            }

            return response;
        }

        /// <summary>
        /// Lógica de negócio do comando, deve ser implementada nos handlers que herdam do base handler
        /// </summary>
        /// <returns>O objeto de retorno é um Tupla com o id da entidade gerada e a entidade em si</returns>
        protected abstract Task<object> ExecuteAsync(TCommand request);
    }
}
