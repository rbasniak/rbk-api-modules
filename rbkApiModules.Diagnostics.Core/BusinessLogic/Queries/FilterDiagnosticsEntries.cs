using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Utilities.Charts.ChartJs;
using System.Linq;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Diagnostics.Core
{
    public class FilterDiagnosticsEntries
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {
                Areas = new string[0];
                Domains = new string[0];
                Agents = new string[0];
                Sources = new string[0];
                Browsers = new string[0];
                Users = new string[0];
                Versions = new string[0];
                Layers = new string[0];
                OperatinSystems = new string[0];
                Devices = new string[0];
                Messages = new string[0];
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string[] Areas { get; set; }
            public string[] Layers { get; set; }
            public string[] Domains { get; set; }
            public string[] Sources { get; set; }
            public string[] Browsers { get; set; }
            public string[] Users { get; set; }
            public string[] Agents { get; set; }
            public string[] Versions { get; set; }
            public string[] OperatinSystems { get; set; }
            public string[] Devices { get; set; }
            public string[] Messages { get; set; }
            public string Text { get; set; }
        }

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly IDiagnosticsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IDiagnosticsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var items = await _context.FilterAsync(request.DateFrom, request.DateTo, request.Versions, request.Areas, request.Layers,
                    request.Domains, request.Sources, request.Users, request.Browsers, request.Agents, request.OperatinSystems,
                    request.Devices, request.Messages, request.Text);

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
                    Id = "sources",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ExceptionSource)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn3, ColorPallete.Autumn4)
                            .WithTitle("Sources")
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
                                .SeriesFrom(x => x.Domain)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn4, ColorPallete.Autumn5)
                            .WithTitle("Areas")
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
                    Id = "devices",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ClientDevice)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn1, ColorPallete.Autumn2)
                            .WithTitle("Devices")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "oss",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ClientOperatingSystem)
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn2, ColorPallete.Autumn3)
                            .WithTitle("Operating Systems")
                                .Chart
                            .WithTooltips()
                                .Chart
                            .Build()
                });

                charts.Add(new ChartDefinition
                {
                    Id = "browsers",
                    Chart = items
                        .CreateRadialChart()
                            .PreparaData()
                                .Take(10)
                                .SeriesFrom(x => x.ClientBrowser.Split('.')[0])
                                .ValueFrom(x => x.Count())
                                .Chart
                            .OfType(ChartType.Doughnut)
                            .Theme(ColorPallete.Autumn3, ColorPallete.Autumn4)
                            .WithTitle("Versions")
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
            public List<DiagnosticsEntry> SearchResults { get; set; }

            public List<ChartDefinition> Charts { get; set; }
        }
    }
}
