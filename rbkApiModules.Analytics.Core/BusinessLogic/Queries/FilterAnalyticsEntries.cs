using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Collections.Generic;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Utilities.Charts.ChartJs;
using System.Linq;

namespace rbkApiModules.Analytics.Core
{
    public class FilterAnalyticsEntries
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {
                Areas = new string[0];
                Domains = new string[0];
                Agents = new string[0];
                Actions = new string[0];
                Responses = new int[0];
                Users = new string[0];
                Versions = new string[0];
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

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly IAnalyticModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IAnalyticModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var items = await _context.FilterAsync(request.DateFrom, request.DateTo, request.Versions, request.Areas, request.Domains, request.Actions,
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

                return results;
            }
        }

        public class Results
        {
            public List<AnalyticsEntry> SearchResults { get; set; }

            public List<ChartDefinition> Charts { get; set; }
        }
    }
}
