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

                var data = await _context.FilterPerformanceData(request.Endpoint, request.DateFrom, request.DateTo);

                var durationData = data.Where(x => x.TransactionCount > 0).OrderBy(x => x.Duration).ToList();
                durationData = durationData.Take((int)(durationData.Count * 0.99)).ToList();

                results.DurationDistribution = BuildDistributionChart(durationData, x => x.Duration, "");   
                results.DurationEvolution = BuildEvolutionChart(durationData, x => x.Duration, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.InSizeDistribution = BuildDistributionChart(data.Where(x => x.RequestSize > 0).ToList(), x => x.RequestSize, "");
                results.InSizeEvolution = BuildEvolutionChart(data.Where(x => x.RequestSize > 0).ToList(), x => x.RequestSize, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.OutSizeDistribution = BuildDistributionChart(data.Where(x => x.ResponseSize > 0).ToList(), x => x.ResponseSize, "");
                results.OutSizeEvolution = BuildEvolutionChart(data.Where(x => x.ResponseSize > 0).ToList(), x => x.ResponseSize, "", request.DateFrom, request.DateTo, request.GroupingType);

                results.TransactionCountDistribution = BuildDistributionChart(data.Where(x => x.TransactionCount > 0).ToList(), x => x.TransactionCount, "");
                results.TransactionCountEvolution = BuildEvolutionChart(data.Where(x => x.TransactionCount > 0).ToList(), x => x.TransactionCount, "", request.DateFrom, request.DateTo, request.GroupingType);

                var databaseTimeData = data.Where(x => x.TransactionCount > 0).OrderBy(x => x.TotalTransactionTime).ToList();
                databaseTimeData = databaseTimeData.Take((int)(databaseTimeData.Count * 0.99)).ToList();

                results.DatabaseDurationDistribution = BuildDistributionChart(databaseTimeData, x => x.TotalTransactionTime, "");
                results.DatabaseDurationEvolution = BuildEvolutionChart(databaseTimeData, x => x.TotalTransactionTime, "", request.DateFrom, request.DateTo, request.GroupingType);

                return results;
            }

            private object BuildEvolutionChart(List<PerformanceEntry> data, Func<PerformanceEntry, double> selector, string title, DateTime from, DateTime to, GroupingType groupingType)
            {
                var chart = data
                    .CreateLinearChart()
                        .PreparaData(groupingType)
                            .EnforceStartDate(from)
                            .EnforceEndDate(to)
                            .DateFrom(x => x.Timestamp)
                            .AddSerie("Minimum", x => { return x.Min(x => selector(x)); })
                            .AddSerie("Average", x => x.Average(x => selector(x)))
                            .AddSerie("Maximum", x => x.Max(x => selector(x)))
                            .Chart
                        .OfType(ChartType.Line)
                        .Responsive() 
                        .WithTooltips()
                            .AtVerticalAxis()
                            .Chart
                        .WithYAxis("x")
                            .AutoSkip(10)
                            .Chart
                        .WithYAxis("y")
                            .Range(0, null)
                            .Chart
                        .SetupDataset("Average")
                            .Color("#01579b")
                            .Thickness(3)
                            .Chart
                        .SetupDataset("Minimum")
                            .Color("#33691e")
                            .PointRadius(2)
                            .Thickness(1)
                            .Chart
                        .SetupDataset("Maximum")
                            .Color("#b71c1c")
                            .PointRadius(2)
                            .Thickness(1)
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildDistributionChart(List<PerformanceEntry> data, Func<PerformanceEntry, double> selector, string title)
            {
                var chart = data
                    .CreateLinearChart()
                        .PreparaData(10)
                            .SetDesiredIntervalFraction(100)
                            .ValuesFrom(x => selector(x))
                            .Chart
                        .OfType(ChartType.Bar)
                        .Theme("77", ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTooltips()
                            .Chart
                        .SetupDataset("default")
                            .OfType(DatasetType.Bar)
                            .RoundedBorders(5)
                            .Thickness(3)
                            .Chart
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
