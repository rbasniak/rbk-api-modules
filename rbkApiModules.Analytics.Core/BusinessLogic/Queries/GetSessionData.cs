using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities;
using rbkApiModules.Utilities.Charts.ChartJs;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Diagnostics.Commons;

namespace rbkApiModules.Analytics.Core
{
    public class GetSessionData
    {
        public class Command : IRequest<QueryResponse>
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public GroupingType GroupingType { get; set; }
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
                var results = new SessionsDashboard();

                var data = await _context.GetSessionsAsync(request.DateFrom, request.DateTo);

                results.TotalDailyUseTime = data
                    .CreateLinearChart()
                        .PreparaData(request.GroupingType)
                            .RoundValues(1)
                            .EnforceStartDate(request.DateFrom)
                            .EnforceEndDate(request.DateTo)
                            .DateFrom(x => x.Start)
                            .SingleSerie()
                            .ValueFrom(x => x.Sum(x => x.Duration) / 60.0)
                            .Chart
                        .OfType(ChartType.Line)
                        .Theme(ColorPallete.Blue2)
                        .Responsive()
                        .WithTitle("Total use time (hours)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .WithYAxis("x")
                            .AutoSkip(10)
                            .Chart
                        .WithYAxis("y")
                            .Range(0, null)
                            .Chart
                        .SetupDatasets()
                            .Thickness(3)
                            .Chart
                        .Build();

                results.TotalAccumulatedUseTime = data
                    .CreateRadialChart()
                        .PreparaData()
                            .RoundValues(1)
                            .SeriesFrom(x => "default")
                            .ValueFrom(x => x.Sum(x => x.Duration) / 60.0)
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                results.DailyUseTimePerUser = data
                    .CreateLinearChart()
                        .PreparaData(request.GroupingType)
                            .Take(10)
                            .EnforceStartDate(request.DateFrom)
                            .EnforceEndDate(request.DateTo)
                            .DateFrom(x => x.Start)
                            .SeriesFrom(x => x.Username)
                            .ValueFrom(x => (int)x.Sum(x => x.Duration))
                            .Chart 
                        .OfType(ChartType.Line)
                        .Theme(ColorPallete.Blue1, ColorPallete.Blue2)
                        .Responsive()
                        .WithTitle("Total use time by user (minutes)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithLegend()
                            .At(PositionType.Bottom)
                            .Align(AlignmentType.Start)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .WithYAxis("x")
                            .AutoSkip(10)
                            .Chart
                        .WithYAxis("y")
                            .Range(0, null)
                            .Chart
                        .SetupDatasets()
                            .Thickness(3)
                            .Chart
                        .Build();

                results.AccumulatedUseTimePerUser = data
                    .CreateRadialChart()
                        .PreparaData()
                            .Take(10)
                            .RoundValues(1)
                            .SeriesFrom(x => x.Username)
                            .ValueFrom(x => x.Sum(x => x.Duration))
                            .Chart
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue1, ColorPallete.Blue2)
                        .Responsive()
                        .Padding(0, 0, 48, 48)
                        .WithTooltips()
                            .Chart
                        .Build();

                results.AverageSessionTime = data
                    .CreateLinearChart()
                        .PreparaData(request.GroupingType)
                            .RoundValues(0)
                            .EnforceStartDate(request.DateFrom)
                            .EnforceEndDate(request.DateTo)
                            .DateFrom(x => x.Start)
                            .SingleSerie()
                            .ValueFrom(x => x.Average(x => x.Duration))
                            .Chart
                        .OfType(ChartType.Line)
                        .SetColors(ColorPallete.Blue1, 2)
                        .Responsive()
                        .WithTitle("Average session duration (minutes)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .WithYAxis("x")
                            .AutoSkip(10)
                            .Chart
                        .WithYAxis("y")
                            .Range(0, null)
                            .Chart
                        .SetupDatasets()
                            .Thickness(3)
                            .Chart
                        .Build();

                results.DailySessions = data
                    .CreateLinearChart()
                        .PreparaData(request.GroupingType)
                            .EnforceStartDate(request.DateFrom)
                            .EnforceEndDate(request.DateTo)
                            .DateFrom(x => x.Start)
                            .SingleSerie()
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Line)
                        .SetColors(ColorPallete.Blue2, 3)
                        .Responsive()
                        .WithTitle("Number of sessions")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .WithYAxis("x")
                            .AutoSkip(10)
                            .Chart
                        .WithYAxis("y")
                            .Range(0, null)
                            .Chart
                        .SetupDatasets()
                            .Thickness(3)
                            .Chart
                        .Build();

                return results;
            }
        }
    }
}
