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

namespace rbkApiModules.Analytics.Core
{
    public class GetPerformanceData
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {

            } 

            public string Endpoint { get; set; }
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
                var results = new PerformanceDashboard();

                var data = await _context.FilterStatisticsAsync(request.DateFrom, request.DateTo, null, null, null, new string[] { }, null, null, new int[] { 200, 201, 204 }, null, 0, null);

                results.DurationDistribution = BuildDistributionChart(data, request.Endpoint, x => x.Duration, "", request.DateFrom, request.DateTo, request.GroupingType);   
                results.DurationEvolution = BuildEvolutionChart(data, request.Endpoint, x => x.Duration, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.InSizeDistribution = BuildDistributionChart(data, request.Endpoint, x => x.RequestSize, "", request.DateFrom, request.DateTo, request.GroupingType);
                results.InSizeEvolution = BuildEvolutionChart(data, request.Endpoint, x => x.RequestSize, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.OutSizeDistribution = BuildDistributionChart(data, request.Endpoint, x => x.ResponseSize, "", request.DateFrom, request.DateTo, request.GroupingType);
                results.OutSizeEvolution = BuildEvolutionChart(data, request.Endpoint, x => x.ResponseSize, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.TransactionCountDistribution = BuildDistributionChart(data, request.Endpoint, x => x.TransactionCount, "", request.DateFrom, request.DateTo, request.GroupingType);
                results.TransactionCountEvolution = BuildEvolutionChart(data, request.Endpoint, x => x.TransactionCount, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.DatabaseDurationDistribution = BuildDistributionChart(data, request.Endpoint, x => x.TotalTransactionTime, "", request.DateFrom, request.DateTo, request.GroupingType);
                results.DatabaseDurationEvolution = BuildEvolutionChart(data, request.Endpoint, x => x.TotalTransactionTime, "", request.DateFrom, request.DateTo, request.GroupingType);

                return results;
            }

            private object BuildEvolutionChart(List<AnalyticsEntry> data, string endpoint, Func<AnalyticsEntry, double> selector, string title, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart = data.CreateLinearChart()
                    .PreparaData(groupingType)
                        .EnforceStartDate(from)
                        .EnforceEndDate(to)
                        .SeriesFrom(x => new [] { x => "Minimum", x => "Maximum", x => "Average" })
                        .DateFrom(x => x.Timestamp)
                        .ValueFrom(x => new [] { x => x.Duration.Min(), x => x.Duration.Max(), x => x.Duration.Average() })
                        .Chart
                    .OfType(ChartType.Line)
                    .Responsive() 
                    .WithTooltips()
                        .Chart
                    .WithYAxis("x")
                        .AutoSkip(10)
                        .Chart
                    .WithYAxis("y")
                        .Range(0, null)
                        .Chart
                    .SetupDataset("Minimum")
                        .Color("#FF0000")
                        .PointRadius(0)
                        .Thickness(3)
                        .Chart
                    .SetupDataset("Maximum")
                        .Color("#FF0000")
                        .PointRadius(0)
                        .Thickness(3)
                        .Chart
                    .SetupDataset("Average")
                        .Color("#00FF00")
                        .PointRadius(0)
                        .Thickness(3)
                        .Chart
                    .Build();

                return chart;
            }

            private object BuildDistributionChart(List<AnalyticsEntry> data, string endpoint, Func<AnalyticsEntry, double> selector, string title, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart = data.CreateLinearChart()
                        .PreparaData()
                            .CategoryFrom(x => x.Timestamp.DayOfWeek.ToString())
                            .SingleSerie()
                            .ValueFrom(x => x.Count())
                            .Chart
                        .OfType(ChartType.Bar)
                        .Theme("77", ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Most active days of the week")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .SetupDataset("default")
                            .OfType(DatasetType.Bar)
                            .RoundedBorders(5)
                            .Thickness(3)
                            .Chart
                        .ReorderCategories(
                            DayOfWeek.Monday.ToString(),
                            DayOfWeek.Tuesday.ToString(),
                            DayOfWeek.Wednesday.ToString(),
                            DayOfWeek.Thursday.ToString(),
                            DayOfWeek.Friday.ToString(),
                            DayOfWeek.Saturday.ToString(),
                            DayOfWeek.Sunday.ToString())
                        .Build();

                return chart;
            }
        }
    }

    class SeriesData
    {
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string Series { get; set; }
    }
}
