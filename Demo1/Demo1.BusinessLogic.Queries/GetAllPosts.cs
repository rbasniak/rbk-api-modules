using MediatR;
using Microsoft.EntityFrameworkCore;
using Demo1.Database.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Queries;

public class GetAllPosts
{
    public class Request: IRequest<QueryResponse> 
    {

    } 

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly ReadDatabaseContext _context;

        public Handler(ReadDatabaseContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            return QueryResponse.Success(await _context.Posts
                .ToListAsync());
        }
    }
}