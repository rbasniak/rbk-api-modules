using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Charts.ChartJs;
using rbkApiModules.Commons.Charts;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Diagnostics.Core;
using Microsoft.AspNetCore.Http.Features;

namespace rbkApiModules.Comments.Core;

public class LoadListData
{
    public class Request : IRequest<QueryResponse>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string[] Levels { get; set; }
        public string[] Messages { get; set; }
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

            var data = await _diagnosticsService.GetAllAsync(startDate, endDate, cancellation);

            var charts = new List<ChartDefinition>
            {
                new ChartDefinition
                {
                    Id = "version",
                    Chart = data
                    .CreateLinearChart()
                        .PreparaData()
                            .SeriesFrom(x => x.Template)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.HorizontalBar)
                        .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                        .WithTitle("Error types")
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build()
                }
            };

            var result = new Response
            {
                Charts = charts.ToArray(),
                Items = data.ToArray()
            };


            return QueryResponse.Success(result);
        }
    }

    public class Response
    {
        public DiagnosticsEntry[] Items { get; set; }

        public ChartDefinition[] Charts { get; set; }
    }
}
