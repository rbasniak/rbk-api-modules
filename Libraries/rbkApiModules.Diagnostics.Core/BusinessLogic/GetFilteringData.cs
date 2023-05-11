using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Diagnostics.Core;

public class GetFilteringLists
{
    public class Command : IRequest<QueryResponse>
    {
    } 

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly IDiagnostricsService _context;

        public Handler(IDiagnostricsService context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var diagnostics = await _context.GetAllAsync(new DateTime(1970, 1, 1), new DateTime(2100, 1, 1), cancellation);

            var data = new FilterOptionListData 
            {
                Levels = diagnostics.Select(d => d.Level).Distinct().Order().ToArray(),
                MessageTemplates = diagnostics.Select(d => d.Template).Distinct().Order().ToArray(),
            };

            return QueryResponse.Success(data);
        } 
    }
}
