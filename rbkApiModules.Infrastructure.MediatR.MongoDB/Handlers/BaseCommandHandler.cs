using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Abstract;
using rbkApiModules.Infrastructure.MediatR.MongoDB;
using System.Runtime.CompilerServices;
using rbkApiModules.Infrastructure.Utilities.MongoDB;

namespace rbkApiModules.Infrastructure.MediatR.MongoDB
{
    /// <summary>
    /// Classe base que todos os handlers devem herdar. Faz o tratamento de exceções automaticamente
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando tratado pelo handler</typeparam>
    public abstract class BaseCommandHandler<TCommand, TDatabase> : BaseHandler<TCommand, CommandResponse>, IRequestHandler<TCommand, CommandResponse>
        where TCommand : IRequest<CommandResponse>
        where TDatabase : MongoDbContext
    {
        protected TDatabase _context;

        public BaseCommandHandler(TDatabase context, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
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

            //using (var session = await _context.Database.Client.StartSessionAsync())
            //{
            //    session.StartTransaction();

                try
                {
                    var result = await ExecuteAsync(request);

                    //await session.CommitTransactionAsync();

                    response.EventData.EntityId = result.entityId;
                    response.Result = result.result;
                }
                catch (SafeException ex)
                {
                    //await session.AbortTransactionAsync();
                    response.AddHandledError(ex.Message);
                }
                catch (Exception ex)
                {
                    //await session.AbortTransactionAsync();
                    response.AddUnhandledError(ex.Message);
                    StoreDiagnosticsData(request, ex);
                }

                return response;
            //}
        }

        /// <summary>
        /// Lógica de negócio do comando, deve ser implementada nos handlers que herdam do base handler
        /// </summary>
        /// <returns>O objeto de retorno é um Tupla com o id da entidade gerada e a entidade em si</returns>
        protected abstract Task<(Guid? entityId, object result)> ExecuteAsync(TCommand request);
    }
}
