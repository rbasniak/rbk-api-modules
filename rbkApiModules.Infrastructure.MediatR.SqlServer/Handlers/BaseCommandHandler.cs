using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Abstract;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Infrastructure.MediatR.SqlServer
{
    /// <summary>
    /// Classe base que todos os handlers devem herdar. Faz o tratamento de exceções automaticamente
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando tratado pelo handler</typeparam>
    public abstract class BaseCommandHandler<TCommand, TDatabase> : BaseHandler<TCommand, CommandResponse>, IRequestHandler<TCommand, CommandResponse> 
        where TCommand : IRequest<CommandResponse>
        where TDatabase : DbContext
    {
        protected TDatabase _context;

        public BaseCommandHandler(TDatabase context, IHttpContextAccessor httpContextAccessor): base(httpContextAccessor)
        {
            _context = context;
        }

        /// <summary>
        /// Método principal do handler, que executa as ações necessárias para processar 
        /// o comando (uso automático do MediatR)
        /// </summary>
        public async Task<CommandResponse> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await ExecuteAsync(request);

                await transaction.CommitAsync();

                response.EventData.EntityId = result.entityId;
                response.Result = result.result;
            }
            catch (SafeException ex)
            {
                response.AddHandledError(ex.Message);
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
        protected abstract Task<(Guid? entityId, object result)> ExecuteAsync(TCommand request);
    }
}
