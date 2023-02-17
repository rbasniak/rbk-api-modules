using MediatR;
using Demo1.Models.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Queries;

public class GetAllBlogs
{
    public class Command: IRequest<QueryResponse> 
    {

    } 

    public class Handler : RequestHandler<Command, QueryResponse>
    {
        private readonly IInMemoryDatabase<Blog> _context;

        public Handler(IInMemoryDatabase<Blog> context)
        {
            _context = context;
        }

        protected override QueryResponse Handle(Command request)
        {
            return QueryResponse.Success( _context.All());
        }
    }
}