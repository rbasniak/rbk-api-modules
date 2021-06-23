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
    public class GetDashboardData
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {

            } 

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
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
                var results = new AnalyticsDashboard();

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo);

                results.AverageTransactionsPerEndpoint = BuildAverageTransactionsPerEndpoint(data, request.DateFrom, request.DateTo);   
                results.BiggestResponsesEndpoints = BuildBiggestResponsesEndpoints(data, request.DateFrom, request.DateTo); 
                results.BiggestResquestsEndpoints = BuildBiggestResquestsEndpoints(data, request.DateFrom, request.DateTo);  
                results.CachedRequestsProportion = BuildCachedRequestsProportion(data, request.DateFrom, request.DateTo);

                results.DailyActiveUsers = BuildDailyActiveUsers(data, request.DateFrom, request.DateTo);  
                results.DailyAuthenticationFailures = BuildDailyAuthenticationFailures(data, request.DateFrom, request.DateTo); 
                results.DailyDatabaseUsage = BuildDailyDatabaseUsage(data, request.DateFrom, request.DateTo);  
                results.DailyErrors = BuildDailyErrors(data, request.DateFrom, request.DateTo);  
                results.DailyInboundTraffic = BuildDailyInboundTraffic(data, request.DateFrom, request.DateTo);  
                results.DailyOutboundTraffic = BuildDailyOutboundTraffic(data, request.DateFrom, request.DateTo);  
                results.DailyRequests = BuildDailyRequests(data, request.DateFrom, request.DateTo);  
                results.DailyTransactions = BuildDailyTransactions(data, request.DateFrom, request.DateTo);  

                results.EndpointErrorRates = BuildEndpointErrorRates(data, request.DateFrom, request.DateTo);  
                results.MostActiveDays = BuildMostActiveDays(data, request.DateFrom, request.DateTo);
                results.MostActiveDomains = BuildMostActiveDomains(data, request.DateFrom, request.DateTo);
                results.MostActiveHours = BuildMostActiveHours(data, request.DateFrom, request.DateTo);
                results.MostActiveUsers = MostActiveUsers(data, request.DateFrom, request.DateTo);  
                results.MostFailedEndpoints = MostFailedEndpoints(data, request.DateFrom, request.DateTo); 
                results.MostResourceHungryEndpoint = MostResourceHungryEndpoint(data, request.DateFrom, request.DateTo);  
                results.MostUsedEndpoints = BuildMostUsedEndpoints(data, request.DateFrom, request.DateTo);
                results.SlowestReadEndpoints = BuildSlowestReadEndpoints(data, request.DateFrom, request.DateTo);
                results.TotalTimeComsumptionPerReadEndpoint = BuildTotalTimeComsumptionPerReadEndpoint(data, request.DateFrom, request.DateTo);

                return results;
            }


            private List<AnalyticsEntry> PrefilterResults(List<AnalyticsEntry> rawData, string[] responses, string[] methods)
            {
                var results = rawData.Where(x => x.Action != null).ToList();

                if (responses != null && responses.Length > 0)
                {
                    results = results.Where(x => responses.Any(response => x.Response.ToString() == response)).ToList();
                }

                if (methods != null && methods.Length > 0)
                {
                    results = results.Where(x => methods.Any(method => x.Method == method)).ToList();
                }

                return results;
            }

            private List<AnalyticsEntry> PrefilterResults(List<AnalyticsEntry> rawData, DateTime from, DateTime to, string[] responses, string[] methods)
            {
                var filteredData = rawData.Where(x => x.Timestamp.Date >= from.Date && x.Timestamp.Date <= to.Date).ToList();

                return PrefilterResults(filteredData, responses, methods);
            }

            private object BuildAverageTransactionsPerEndpoint(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, Math.Round(x.Average(x => x.TransactionCount), 1)))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Average transactions per endpoint")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildBiggestResponsesEndpoints(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, new[] { "200", "204" }, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, Math.Round(x.Average(x => x.ResponseSize), 1)))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Biggest response sizes")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .RoundToNearestStorageUnit()
                        .Build();

                return chart;
            }

            private object BuildBiggestResquestsEndpoints(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "200", "204" }, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, Math.Round(x.Average(x => x.RequestSize), 1)))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Biggest request sizes")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .RoundToNearestStorageUnit()     
                        .Build();

                return chart;
            }

            private object BuildCachedRequestsProportion(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "200", "204" }, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, x.Count(y => y.WasCached) / (double)x.Count() * 100.0))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Cached response proportions (%)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildDailyActiveUsers(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, null, null)
                        .GroupBy(x => x.Timestamp.Date)
                        .Select(x => new NeutralDatePoint("default", x.Key, x.GroupBy(x => x.Username).Count()))
                    .CreateLinearChart(GroupingType.Daily, false, from, to)
                    .OfType(ChartType.Line)
                    .Theme(ColorPallete.Blue2)
                    .Responsive()
                    .WithTitle("Daily active users")
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
                    .SetupDataset("default")
                        .Thickness(3)
                        .Chart
                    .Build();

                return chart;
            }

            private object BuildDailyAuthenticationFailures(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "401", "403" }, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Count()))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily authentication failures")
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
                .SetupDataset("default")
                    .Thickness(3)
                    .Chart
                .Build();

                return chart;
            }

            private object BuildDailyDatabaseUsage(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Sum(x => x.TotalTransactionTime)))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily database usage (total seconds)")
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
                .SetupDataset("default")
                    .Thickness(3)
                    .Chart
                .Build();

                return chart;
            }

            private object BuildDailyErrors(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "500" }, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Count()))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily errors")
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
                .SetupDataset("default")
                    .Thickness(3)
                    .Chart
                .Build();

                return chart;
            }

            private object BuildDailyInboundTraffic(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Sum(x => x.RequestSize)))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily inbound traffic")
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
                .SetupDataset("default")
                    .RoundToNearestStorageUnit(true)
                    .Thickness(3)
                    .Chart
                .Build(); 

                return chart;
            }

            private object BuildDailyOutboundTraffic(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Sum(x => x.ResponseSize)))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily outbound traffic")
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
                .SetupDataset("default")
                    .RoundToNearestStorageUnit(true)
                    .Thickness(3)
                    .Chart
                .Build();

                return chart;
            }

            private object BuildDailyRequests(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Count()))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily API calls")
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
                .SetupDataset("default")
                    .Thickness(3)
                    .Chart
                .Build();

                return chart;
            }

            private object BuildDailyTransactions(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => new NeutralDatePoint("default", x.Key, x.Sum(x => x.TransactionCount)))
                .CreateLinearChart(GroupingType.Daily, false, from, to)
                .OfType(ChartType.Line)
                .Theme(ColorPallete.Blue2)
                .Responsive()
                .WithTitle("Daily database transactions")
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
                .SetupDataset("default")
                    .Thickness(3)
                    .Chart
                .Build();

                return chart;
            }

            private object BuildEndpointErrorRates(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Action)
                    .Where(x => x.Key != null)
                    .Select(x => 
                    {
                        var success = (double)x.Count(y => y.Response == 200 || y.Response == 204);
                        var errors = (double)x.Count(y => y.Response != 200 && y.Response != 204 && y.Response != 400 && y.Response != 401 && y.Response != 403);

                        return new NeutralCategoryPoint("default", x.Key, errors / (success + errors) * 100.0);
                    })
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue1, ColorPallete.Blue2)
                        .Responsive()
                        .WithTitle("Endpoint error rates")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildMostActiveDays(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.DayOfWeek.ToString())
                    .Select(x => new NeutralCategoryPoint("default", x.Key, x.Count()))
                    .CreateLinearChart()
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
                        .Build();

                return chart;
            }

            private object BuildMostActiveHours(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Timestamp.Hour)
                    .OrderBy(x => x.Key)
                    .Select(x => new NeutralCategoryPoint("default", x.Key.ToString("00"), x.Count()))
                    .CreateLinearChart()
                        .OfType(ChartType.Bar)
                        .SetColors(new string[] { "#345DB3" }, "77")
                        .Responsive()
                        .WithTitle("Most active hours of the day")
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
                        .Build();

                return chart;
            }

            private object BuildMostActiveDomains(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Domain)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, x.Count()))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Most active domains")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;

            }

            private object MostActiveUsers(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Username)
                    .Select(x => new NeutralCategoryPoint("default", String.IsNullOrEmpty(x.Key) ? "Anonymous" : x.Key, x.Count()))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Most active users")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object MostFailedEndpoints(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "500" }, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, x.Count()))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Most failed endpoints")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object MostResourceHungryEndpoint(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, Math.Round(x.Average(y => y.TotalTransactionTime), 0)))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Total database time comsumption (seconds)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildMostUsedEndpoints(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, null, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, x.Count()))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Most used endpoints")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildSlowestReadEndpoints(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "200", "204" }, null)
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, Math.Round(x.Average(y => y.Duration), 0)))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Slowest endpoints (average, seconds)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }

            private object BuildTotalTimeComsumptionPerReadEndpoint(List<AnalyticsEntry> data, DateTime from, DateTime to)
            { 
                var chart = PrefilterResults(data, from, to, new[] { "200", "204" }, new[] { "GET" })
                    .GroupBy(x => x.Action)
                    .Select(x => new NeutralCategoryPoint("default", x.Key, x.Sum(y => y.Duration)))
                    .Take(10)
                    .CreateRadialChart(false)
                        .OfType(ChartType.Doughnut)
                        .Theme(ColorPallete.Blue2, ColorPallete.Blue1)
                        .Responsive()
                        .WithTitle("Total time comsumption per endpoint (seconds)")
                            .Font(16)
                            .Padding(8, 24)
                            .Chart
                        .WithTooltips()
                            .Chart
                        .Build();

                return chart;
            }
        }
    }
}
