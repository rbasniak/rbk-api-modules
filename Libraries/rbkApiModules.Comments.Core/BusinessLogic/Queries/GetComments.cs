using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

public class GetComments
{
    public class Command : AuthenticatedRequest, IRequest<QueryResponse>
    {
        public Guid EntityId { get; set; }
    } 

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly ICommentsService _commentsService;

        public Handler(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            return QueryResponse.Success(await _commentsService.GetAllAsync(request.Identity.Tenant, request.EntityId, cancellation));
        }
    }
}
