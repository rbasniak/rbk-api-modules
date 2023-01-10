using MediatR;
using Microsoft.EntityFrameworkCore;
using Demo1.Database.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Queries;

public class GetAllPosts
{
    public class Command: IRequest<QueryResponse> 
    {

    } 

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly ReadDatabaseContext _context;

        public Handler(ReadDatabaseContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            return QueryResponse.Success(await _context.Posts
                .ToListAsync());
        }
    }
}