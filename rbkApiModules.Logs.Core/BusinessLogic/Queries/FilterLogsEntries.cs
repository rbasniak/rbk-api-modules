using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Utilities.Charts.ChartJs;

namespace rbkApiModules.Logs.Core
{
    public class FilterLogsEntries
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {
                Messages = new string[0];
                Levels = new LogLevel[0];
                Layers = new string[0];
                Areas = new string[0];
                Versions = new string[0];
                Sources = new string[0];
                Enviroments = new string[0];
                EnviromentsVersions = new string[0];
                Users = new string[0];
                Domains = new string[0];
                Machines = new string[0];
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string[] Messages { get; set; }
            public LogLevel[] Levels { get; set; }
            public string[] Layers { get; set; }
            public string[] Areas { get; set; }
            public string[] Versions { get; set; }
            public string[] Sources { get; set; }
            public string[] Enviroments { get; set; }
            public string[] EnviromentsVersions { get; set; }
            public string[] Users { get; set; }
            public string[] Domains { get; set; }
            public string[] Machines { get; set; }
        }

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly ILogsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, ILogsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var items = await _context.FilterAsync(request.DateFrom, request.DateTo, request.Messages, request.Levels, request.Layers, request.Areas,
                    request.Versions, request.Sources, request.Enviroments, request.EnviromentsVersions, request.Users, request.Domains, request.Machines);

                var charts = new List<ChartDefinition>();

                charts.Add(new ChartDefinition 
                {
                    Id = "version",
                    Chart = items 
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ApplicationVersion)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                            .WithTitle("Versions")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "layers",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ApplicationLayer)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn2, ColorPallete.Autumn3)
                            .WithTitle("Layers")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "areas",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ApplicationArea)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn3, ColorPallete.Autumn4)
                            .WithTitle("Areas")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "sources",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.Source)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn4, ColorPallete.Autumn5)
                            .WithTitle("Sources")
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
                                .SeriesFrom(x => x.Username)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn5, ColorPallete.Autumn1)
                            .WithTitle("Users")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "domains",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.Domain)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                            .WithTitle("Domains")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "machines",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.MachineName)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn2, ColorPallete.Autumn3)
                            .WithTitle("Machines names")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "enviroments",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.Enviroment)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn3, ColorPallete.Autumn4)
                            .WithTitle("Enviroments")
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

                return results;
            }
        }

        public class Results
        {
            public List<LogEntry> SearchResults { get; set; }

            public List<ChartDefinition> Charts { get; set; }
        }
    }
}
