using MediatR;
using Microsoft.EntityFrameworkCore;
using Demo1.Database.Domain;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Queries;

public class GetAllAuthors
{
    public class Command: IRequest<QueryResponse> 
    {

    } 

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            return QueryResponse.Success(await _context.Authors.ToListAsync());
        }
    }
}