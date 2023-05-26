using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Diagnostics.Core;

public class GetFilteringLists
{
    public class Request : IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IDiagnostricsService _context;

        public Handler(IDiagnostricsService context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var diagnostics = await _context.GetAllAsync(new DateTime(1970, 1, 1), new DateTime(2100, 1, 1), cancellation);

            var firstEntry = diagnostics.Min(x => x.Timestamp);

            var data = new Response
            {
                MinimumDate = firstEntry,
                Levels = diagnostics.Select(d => d.Level).Distinct().Order().ToArray(),
                MessageTemplates = diagnostics.Select(d => d.Template).Distinct().Order().ToArray(),
            };

            return QueryResponse.Success(data);
        }
    }

    public class Response
    {
        public Response()
        {
            Levels = Array.Empty<string>();
            MessageTemplates = Array.Empty<string>();
        }

        public DateTime MinimumDate { get; set; }
        public string[] Levels { get; set; }
        public string[] MessageTemplates { get; set; }
    }
}
