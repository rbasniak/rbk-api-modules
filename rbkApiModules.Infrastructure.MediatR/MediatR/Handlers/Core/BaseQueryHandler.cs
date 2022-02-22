using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Abstract;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    public abstract class BaseQueryHandler<TCommand> : BaseHandler<TCommand, QueryResponse>, IRequestHandler<TCommand, QueryResponse>
        where TCommand : IRequest<QueryResponse>
    {
        public BaseQueryHandler(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<QueryResponse> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var response = new QueryResponse();

            try
            {
                var result = await ExecuteAsync(request);

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

        protected abstract Task<object> ExecuteAsync(TCommand request);
    }
}