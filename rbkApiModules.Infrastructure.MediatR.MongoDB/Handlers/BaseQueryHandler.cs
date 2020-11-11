using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Abstract;
using rbkApiModules.Infrastructure.Utilities.MongoDB;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Infrastructure.MediatR.MongoDB
{
    public abstract class BaseQueryHandler<TCommand, TDatabase> : BaseHandler<TCommand, QueryResponse>, IRequestHandler<TCommand, QueryResponse>
        where TCommand : IRequest<QueryResponse>
        where TDatabase : MongoDbContext
    {
        protected readonly TDatabase _context;

        public BaseQueryHandler(TDatabase context, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
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
                StoreDiagnosticsData(request, ex);
            }

            return response;
        }

        protected abstract Task<object> ExecuteAsync(TCommand request);
    }
}
