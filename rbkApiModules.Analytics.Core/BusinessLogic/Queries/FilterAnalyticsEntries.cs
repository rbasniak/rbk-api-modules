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
                        .GroupBy(x => x.Version)
                        .OrderBy(x => x.Count())
                        .Take(10)
                        .Select(x => new NeutralCategoryPoint(x.Key, x.Count()))
                        .CreateRadialChart(10, "Others", false)
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
                        .GroupBy(x => x.Username)
                        .OrderBy(x => x.Count())
                        .Take(10)
                        .Select(x => new NeutralCategoryPoint(x.Key, x.Count()))
                        .CreateRadialChart(10, "Others", false)
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
                        .GroupBy(x => x.Response)
                        .OrderBy(x => x.Count())
                        .Take(10)
                        .Select(x => new NeutralCategoryPoint(x.Key.ToString(), x.Count()))
                        .CreateRadialChart(10, "Others", false)
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
                        .GroupBy(x => x.Action)
                        .OrderBy(x => x.Count())
                        .Take(10)
                        .Select(x => new NeutralCategoryPoint(x.Key.ToString(), x.Count()))
                        .CreateRadialChart(10, "Others", false)
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
