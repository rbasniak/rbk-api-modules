using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Charts;
using rbkApiModules.Commons.Charts.ChartJs;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Analytics;

public class GetPerformanceData
{
    public class Request : IRequest<QueryResponse>
    {
        public Request()
        {

        }

        public string Endpoint { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public GroupingType GroupingType { get; set; }
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IAnalyticModuleStore _context;

        public Handler(IAnalyticModuleStore context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var results = new PerformanceDashboard();

            var data = await _context.FilterPerformanceData(request.Endpoint, request.DateFrom, request.DateTo);

            var durationData = data
                .Where(x => x.TransactionCount > 0 && !x.HasError)
                .OrderBy(x => x.Duration).ToList();
            durationData = durationData.Take((int)(durationData.Count * 0.99)).ToList();

            results.DailyUsage = BuildDailyUsageChart(durationData, request.DateFrom, request.DateTo, request.GroupingType);
            results.UserUsage = BuildUserUsageChart(durationData);

            results.DurationDistribution = BuildDistributionChart(durationData, x => x.Duration, "");
            results.DurationEvolution = BuildEvolutionChart(durationData, x => x.Duration, "", request.DateFrom, request.DateTo, request.GroupingType);

            results.InSizeDistribution = BuildDistributionChart(data.Where(x => x.RequestSize > 0 && !x.HasError).ToList(), x => x.RequestSize, "");
            results.InSizeEvolution = BuildEvolutionChart(data.Where(x => x.RequestSize > 0 && !x.HasError).ToList(), x => x.RequestSize, "", request.DateFrom, request.DateTo, request.GroupingType);

            results.OutSizeDistribution = BuildDistributionChart(data.Where(x => x.ResponseSize > 0 && !x.HasError).ToList(), x => x.ResponseSize, "");
            results.OutSizeEvolution = BuildEvolutionChart(data.Where(x => x.ResponseSize > 0 && !x.HasError).ToList(), x => x.ResponseSize, "", request.DateFrom, request.DateTo, request.GroupingType);

            results.TransactionCountDistribution = BuildDistributionChart(data.Where(x => x.TransactionCount > 0 && !x.HasError).ToList(), x => x.TransactionCount, "");
            results.TransactionCountEvolution = BuildEvolutionChart(data.Where(x => x.TransactionCount > 0 && !x.HasError).ToList(), x => x.TransactionCount, "", request.DateFrom, request.DateTo, request.GroupingType);

            var databaseTimeData = data.Where(x => x.TransactionCount > 0 && !x.HasError).OrderBy(x => x.TotalTransactionTime).ToList();
            databaseTimeData = databaseTimeData.Take((int)(databaseTimeData.Count * 0.99)).ToList();

            results.DatabaseDurationDistribution = BuildDistributionChart(databaseTimeData, x => x.TotalTransactionTime, "");
            results.DatabaseDurationEvolution = BuildEvolutionChart(databaseTimeData, x => x.TotalTransactionTime, "", request.DateFrom, request.DateTo, request.GroupingType);

            return QueryResponse.Success(results);
        }

        private object BuildEvolutionChart(List<PerformanceEntry> data, Func<PerformanceEntry, double> selector, string title, DateTime from, DateTime to, GroupingType groupingType)
        {
            var chart = data
                .CreateLinearChart()
                    .PreparaData(groupingType)
                        .EnforceStartDate(from)
                        .EnforceEndDate(to)
                        .DateFrom(x => x.Timestamp)
                        .AddSerie("Minimum", x => x.Min(x => selector(x)))
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

        private object BuildDailyUsageChart(List<PerformanceEntry> data, DateTime from, DateTime to, GroupingType groupingType)
        {
            var chart = data
                .CreateLinearChart()
                    .PreparaData(groupingType)
                        .EnforceStartDate(from)
                        .EnforceEndDate(to)
                        .DateFrom(x => x.Timestamp)
                        .AddSerie("Hits", x => x.Count())
                        .AddSerie("Errors", x => x.Where(x => x.HasError).Count())
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
                    .SetupDataset("Hits")
                        .Color("#01579b")
                        .Thickness(3)
                        .Chart
                    .SetupDataset("Errors")
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

        private object BuildUserUsageChart(List<PerformanceEntry> data)
        {
            var chart = data
                .CreateRadialChart()
                    .PreparaData()
                        .RoundValues(1)
                        .SeriesFrom(x => x.Username)
                        //.MaximumSeries(10, "Outros")
                        .Take(10)
                        .ValueFrom(x => x.Count())
                        .Chart
                    .OfType(ChartType.Doughnut)
                    .Theme(ColorPallete.Bright1, ColorPallete.Bright2)
                    .Responsive()
                    // .Padding(0, 0, 48, 48)
                    .WithTooltips()
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