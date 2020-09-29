using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Infrastructure.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR
{
    /// <summary>
    /// Classe base que todos os handlers devem herdar. Faz o tratamento de exceções automaticamente
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando tratado pelo handler</typeparam>
    public abstract class BaseCommandHandler<TCommand, TDatabase> : IRequestHandler<TCommand, CommandResponse> 
        where TCommand : IRequest<CommandResponse>
        where TDatabase : DbContext
    {
        protected TDatabase _context;
        protected IHttpContextAccessor _httpContextAccessor;
        private readonly IDiagnosticsModuleStore _diagnosticsStore;

        public BaseCommandHandler(TDatabase context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _diagnosticsStore = httpContextAccessor?.HttpContext.RequestServices.GetService<IDiagnosticsModuleStore>();
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

                if (_httpContextAccessor != null)
                {
                    var exceptionData = new DiagnosticsEntry(_httpContextAccessor.HttpContext, request.GetType().FullName, ex, request);
                    _diagnosticsStore.StoreData(exceptionData);
                }
            }

            return response;
        }

        /// <summary>
        /// Lógica de negócio do comando, deve ser implementada nos handlers que herdam do base handler
        /// </summary>
        /// <returns>O objeto de retorno é um Tupla com o id da entidade gerada e a entidade em si</returns>
        protected abstract Task<(Guid? entityId, object result)> ExecuteAsync(TCommand request);
    }

    /// <summary>
    /// Classe base que todos os handlers devem herdar. Faz o tratamento de exceções automaticamente
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando tratado pelo handler</typeparam>
    public abstract class BaseCommandHandler<TCommand> : IRequestHandler<TCommand, CommandResponse>
        where TCommand : IRequest<CommandResponse>
    {
        protected IHttpContextAccessor _httpContextAccessor;
        private readonly IDiagnosticsModuleStore _diagnosticsStore;

        public BaseCommandHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _diagnosticsStore = httpContextAccessor?.HttpContext.RequestServices.GetService<IDiagnosticsModuleStore>();
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

                response.Result = result;
            }
            catch (SafeException ex)
            {
                response.AddHandledError(ex.Message);
            }
            catch (Exception ex)
            {
                response.AddUnhandledError(ex.Message);

                if (_httpContextAccessor != null)
                {
                    var exceptionData = new DiagnosticsEntry(_httpContextAccessor.HttpContext, request.GetType().FullName, ex, request);
                    _diagnosticsStore.StoreData(exceptionData);
                }
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
