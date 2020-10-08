using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Infrastructure.MediatR
{
    public abstract class BaseQueryHandler<TCommand, TDatabase> : IRequestHandler<TCommand, QueryResponse> 
        where TCommand : IRequest<QueryResponse>
        where TDatabase : DbContext
    {
        protected readonly TDatabase _context;
        protected IHttpContextAccessor _httpContextAccessor;
        private readonly IDiagnosticsModuleStore _diagnosticsStore;

        public BaseQueryHandler(TDatabase context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<QueryResponse> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var response = new QueryResponse();

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

                var diagnosticsStore = _httpContextAccessor?.HttpContext?.RequestServices?.GetService<IDiagnosticsModuleStore>();

                if (diagnosticsStore != null)
                {
                    var exceptionData = new DiagnosticsEntry(_httpContextAccessor.HttpContext, request.GetType().FullName, ex, request);
                    diagnosticsStore.StoreData(exceptionData);
                }
            }

            return response;
        }

        protected abstract Task<object> ExecuteAsync(TCommand request);
    }

    public abstract class BaseQueryHandler<TCommand> : IRequestHandler<TCommand, QueryResponse>
        where TCommand : IRequest<QueryResponse>
    {
        protected IHttpContextAccessor _httpContextAccessor;
        private readonly IDiagnosticsModuleStore _diagnosticsStore;

        public BaseQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _diagnosticsStore = httpContextAccessor?.HttpContext.RequestServices.GetService<IDiagnosticsModuleStore>();
        }

        public async Task<QueryResponse> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var response = new QueryResponse();

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
                var diagnosticsStore = _httpContextAccessor?.HttpContext?.RequestServices?.GetService<IDiagnosticsModuleStore>();

                if (diagnosticsStore != null)
                {
                    var exceptionData = new DiagnosticsEntry(_httpContextAccessor.HttpContext, request.GetType().FullName, ex, request);
                    diagnosticsStore.StoreData(exceptionData);
                }
            }

            return response;
        }

        protected abstract Task<object> ExecuteAsync(TCommand request);
    }
}
