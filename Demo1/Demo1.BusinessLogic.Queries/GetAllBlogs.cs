using MediatR;
using Demo1.Models.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Queries;

public class GetAllBlogs
{
    public class Request: IRequest<QueryResponse> 
    {

    } 

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IInMemoryDatabase<Blog> _context;

        //public Handler(IInMemoryDatabase<Blog> context)
        //{
        //    _context = context;
        //}

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(QueryResponse.Success( _context.All()));
        }
    }
}