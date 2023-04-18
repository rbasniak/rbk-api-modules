using MediatR;
using rbkApiModules.Commons.Charts;
using rbkApiModules.Commons.Charts.ChartJs;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Localization;

namespace rbkApiModules.Commons.Analytics;

public class FilterAnalyticsEntries
{
    public class Request : IRequest<QueryResponse>
    {
        public Request()
        {
            Areas = Array.Empty<string>();
            Domains = Array.Empty<string>();
            Agents = Array.Empty<string>();
            Actions = Array.Empty<string>();
            Responses = Array.Empty<int>();
            Users = Array.Empty<string>();
            Versions = Array.Empty<string>();
        }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string[] Areas { get; set; }
        public string[] Domains { get; set; }
        public string[] Actions { get; set; }
        public int[] Responses { get; set; }
        public string[] Users { get; set; }
        public string[] Agents { get; set; }
        public string[] Versions { get; set; }
        public int Duration { get; set; }
        public string EntityId { get; set; }
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IAnalyticModuleStore _context;

        public Handler(IAnalyticModuleStore context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var items = await _context.FilterStatisticsAsync(request.DateFrom, request.DateTo, request.Versions, request.Areas, request.Domains, request.Actions,
                request.Users, request.Agents, request.Responses, null, request.Duration, request.EntityId);

            var charts = new List<ChartDefinition>();

            charts.Add(new ChartDefinition
            {
                Id = "versions",
                Chart = items
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .RoundValues(1)
                            .SeriesFrom(x => x.Version)
                            .ValueFrom(x => x.Count())
                            .AppendExtraData()
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue1, ColorPallete.Blue2)
                        .WithTitle("Versions")
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build()
            });

            charts.Add(new ChartDefinition
            {
                Id = "users",
                Chart = items
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .RoundValues(1)
                            .SeriesFrom(x => x.Username)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .WithTitle("Users")
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build()
            });

            charts.Add(new ChartDefinition
            {
                Id = "responses",
                Chart = items
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .RoundValues(1)
                            .SeriesFrom(x => x.Response.ToString())
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue1, ColorPallete.Blue2)
                        .WithTitle("Responses")
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build()
            });

            charts.Add(new ChartDefinition
            {
                Id = "actions",
                Chart = items
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .RoundValues(1)
                            .SeriesFrom(x => x.Action)
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .WithTitle("Endpoints")
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build()
            });

            var results = new Results
            {
                SearchResults = items,
                Charts = charts
            };

            return QueryResponse.Success(results);
        }

    }

    public class Results
    {
        public List<AnalyticsEntry> SearchResults { get; set; }

        public List<ChartDefinition> Charts { get; set; }
    }
}
