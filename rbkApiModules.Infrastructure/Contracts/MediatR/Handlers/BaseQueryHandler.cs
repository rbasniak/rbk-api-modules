using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR
{
    public abstract class BaseQueryHandler<TCommand, TDatabase> : IRequestHandler<TCommand, QueryResponse> 
        where TCommand : IRequest<QueryResponse>
        where TDatabase : DbContext
    {
        protected readonly TDatabase _context;
        protected IHttpContextAccessor _httpContextAccessor;

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
            }

            return response;
        }

        protected abstract Task<object> ExecuteAsync(TCommand request);
    }
}
