using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Diagnostics.Core;

namespace rbkApiModules.Comments.Core;

public class GetDashboardData
{
    public class Request : IRequest<QueryResponse>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    } 

    public class Validator : AbstractValidator<Request>
    {
        public Validator(ILocalizationService localization)
        {
            RuleFor(x => x.EndDate)
                .Must(BeGreaterOrEqual)
                .WithMessage(localization.LocalizeString(DiagnosticsMessages.Validation.EndDateMustBeGreaterOrEqualToStartDate));
        }

        private bool BeGreaterOrEqual(Request request, DateTime? _)
        {
            if (request.StartDate == null || request.EndDate == null)
            {
                return true;
            }

            return request.EndDate.Value >= request.StartDate.Value;
        }
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IDiagnostricsService _diagnosticsService;

        public Handler(IDiagnostricsService diagnosticsService)
        {
            _diagnosticsService = diagnosticsService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var startDate = request.StartDate.HasValue ? new DateTime(request.StartDate.Value.Year, request.StartDate.Value.Month, request.StartDate.Value.Day, 0, 0, 0) : new DateTime(1970, 1, 1);
            var endDate = request.EndDate.HasValue ? new DateTime(request.EndDate.Value.Year, request.EndDate.Value.Month, request.EndDate.Value.Day, 23, 59, 59) : new DateTime(2100, 1, 1);

            return QueryResponse.Success(await _diagnosticsService.GetAllAsync(startDate, endDate, cancellation));
        }
    }

    public class Response
    {

    }
}
