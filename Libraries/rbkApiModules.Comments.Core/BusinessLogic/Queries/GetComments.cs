using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

public class GetComments
{
    public class Request : AuthenticatedRequest, IRequest<QueryResponse>
    {
        public Guid EntityId { get; set; }
    } 

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly ICommentsService _commentsService;

        public Handler(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            return QueryResponse.Success(await _commentsService.GetAllAsync(request.Identity.Tenant, request.EntityId, cancellation));
        }
    }
}
