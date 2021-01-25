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

namespace rbkApiModules.Analytics.Core
{
    public class GetDashboardData
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {

            }

            public Command(DateTime from, DateTime to)
            {
                DateFrom = from;
                DateTo = to;
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                CascadeMode = CascadeMode.Stop;
            }
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
                var results = new AnalyticsResults();

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo);

                results.AverageTransactionsPerEndpoint = BuildAverageTransactionsPerEndpoint(data); // ok
                results.BiggestResponsesEndpoints = BuildBiggestResponsesEndpoints(data);
                results.BiggestResquestsEndpoints = BuildBiggestResquestsEndpoints(data);
                results.CachedRequestsProportion = BuildCachedRequestsProportion(data);

                results.DailyActiveUsers = BuildDailyActiveUsers(data, request.DateFrom, request.DateTo);
                results.DailyAuthenticationFailures = BuildDailyAuthenticationFailures(data, request.DateFrom, request.DateTo);
                results.DailyDatabaseUsage = BuildDailyDatabaseUsage(data, request.DateFrom, request.DateTo);
                results.DailyErrors = BuildDailyErrors(data, request.DateFrom, request.DateTo);
                results.DailyInboundTraffic = BuildDailyInboundTraffic(data, request.DateFrom, request.DateTo);
                results.DailyOutboundTraffic = BuildDailyOutboundTraffic(data, request.DateFrom, request.DateTo);
                results.DailyRequests = BuildDailyRequests(data, request.DateFrom, request.DateTo);
                results.DailyTransactions = BuildDailyTransactions(data, request.DateFrom, request.DateTo);

                results.EndpointErrorRates = BuildEndpointErrorRates(data);
                results.MostActiveDays = BuildMostActiveDays(data);
                results.MostActiveDomains = BuildMostActiveDomains(data);
                results.MostActiveHours = BuildMostActiveHours(data);
                results.MostActiveUsers = MostActiveUsers(data);
                results.MostFailedEndpoints = MostFailedEndpoints(data);
                results.MostResourceHungryEndpoint = MostResourceHungryEndpoint(data);
                results.MostUsedEndpoints = BuildMostUsedEndpoints(data);
                results.SlowestReadEndpoints = BuildSlowestReadEndpoints(data);
                results.SlowestWriteEndpoints = BuildSlowestWriteEndpoints(data);
                results.TotalTimeComsumptionPerReadEndpoint = BuildTotalTimeComsumptionPerReadEndpoint(data);
                results.TotalTimeComsumptionPerWriteEndpoint = BuildTotalTimeComsumptionPerWriteEndpoint(data);

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

            private List<SimpleLabeledValue<int>> BuildAverageTransactionsPerEndpoint(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, (int)itemData.Average(x => x.TransactionCount)));
                }

                results = results.OrderByDescending(x => x.Value).ToList();

                return results;
            }

            private List<SimpleLabeledValue<int>> BuildBiggestResponsesEndpoints(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = itemData.Key.ToString();

                    results.Add(new SimpleLabeledValue<int>(name, (int)itemData.Average(x => x.ResponseSize)));
                }

                results = results.OrderByDescending(x => x.Value).ToList();

                return results;
            }

            private List<SimpleLabeledValue<int>> BuildBiggestResquestsEndpoints(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = itemData.Key.ToString();

                    results.Add(new SimpleLabeledValue<int>(name, (int)itemData.Average(x => x.RequestSize)));
                }

                results = results.OrderByDescending(x => x.Value).ToList();

                return results;
            }

            private List<SimpleLabeledValue<double>> BuildCachedRequestsProportion(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<double>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = itemData.Key.ToString();

                    var cached = itemData.Count(x => x.WasCached);
                    var total = (double)itemData.Count();

                    results.Add(new SimpleLabeledValue<double>(name, cached / total * 100.0));
                }

                results = results.OrderByDescending(x => x.Value).ToList();

                return results;
            }

            private List<DateValuePoint> BuildDailyActiveUsers(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var activeUsers = itemData.GroupBy(x => x.Username).Count();

                    var point = chartData.First(x => x.Date == date);

                    point.Value = activeUsers;
                }

                return chartData;
            }

            private List<DateValuePoint> BuildDailyAuthenticationFailures(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, new[] { "401", "403" }, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var value = itemData.Count();

                    var point = chartData.First(x => x.Date == date);

                    point.Value = value;
                }

                return chartData;
            }

            private List<DateValuePoint> BuildDailyDatabaseUsage(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var value = itemData.Sum(x => x.TotalTransactionTime);

                    var point = chartData.First(x => x.Date == date);

                    point.Value = value;
                }

                return chartData;
            }


            private List<DateValuePoint> BuildDailyErrors(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, new[] { "500" }, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var errors = itemData.Count();

                    var point = chartData.First(x => x.Date == date);

                    point.Value = errors;
                }

                return chartData;
            }

            private List<DateValuePoint> BuildDailyInboundTraffic(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var totalSize = itemData.Sum(x => x.RequestSize);

                    var point = chartData.First(x => x.Date == date);

                    // TODO: passar a unidade como configuração da lib
                    point.Value = totalSize / 1024;
                }

                return chartData;
            }

            private List<DateValuePoint> BuildDailyOutboundTraffic(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var totalSize = itemData.Sum(x => x.ResponseSize);

                    var point = chartData.First(x => x.Date == date);

                    // TODO: passar a unidade como configuração da lib
                    point.Value = totalSize / 1024;
                }

                return chartData;
            }

            private List<DateValuePoint> BuildDailyRequests(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var value = itemData.Count();

                    var point = chartData.First(x => x.Date == date);

                    point.Value = value;
                }

                return chartData;
            }

            private List<DateValuePoint> BuildDailyTransactions(List<AnalyticsEntry> data, DateTime from, DateTime to)
            {
                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(from, to);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var value = itemData.Sum(x => x.TransactionCount);

                    var point = chartData.First(x => x.Date == date);

                    point.Value = value;
                }

                return chartData;

            }

            private List<SimpleLabeledValue<double>> BuildEndpointErrorRates(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<double>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    if (itemData.Key == null) continue;

                    var name = itemData.Key.ToString();

                    var success = (double)itemData.Count(x => x.Response == 200 || x.Response == 204);
                    var errors = (double)itemData.Count(x => x.Response != 200 && x.Response != 204 && x.Response != 400 && x.Response != 401 && x.Response != 403);

                    results.Add(new SimpleLabeledValue<double>(name, errors / (success + errors) * 100.0));
                }

                return results.OrderByDescending(x => x.Value).ToList();

            }

            private List<SimpleLabeledValue<int>> BuildMostActiveDays(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.DayOfWeek).ToList();

                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Sunday.ToString(), 0));
                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Monday.ToString(), 0));
                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Tuesday.ToString(), 0));
                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Wednesday.ToString(), 0));
                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Thursday.ToString(), 0));
                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Friday.ToString(), 0));
                results.Add(new SimpleLabeledValue<int>(DayOfWeek.Saturday.ToString(), 0));

                foreach (var itemData in groupedData)
                {
                    var element = results.First(x => x.Label == itemData.Key.ToString());
                    element.Value = itemData.Count();
                }

                return results.ToList();
            }
            private List<SimpleLabeledValue<int>> BuildMostActiveDomains(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Domain).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = "Without Domain";

                    if (itemData.Key != null)
                    {
                        name = itemData.Key.ToString();
                    }

                    results.Add(new SimpleLabeledValue<int>(name, itemData.Count()));
                }

                return results.OrderByDescending(x => x.Value).ToList();

            }

            private List<SimpleLabeledValue<int>> BuildMostActiveHours(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Timestamp.Hour).ToList();

                for (int i = 0; i < 24; i++)
                {
                    results.Add(new SimpleLabeledValue<int>(i.ToString("00"), 0));
                }

                foreach (var itemData in groupedData)
                {
                    var element = results.First(x => x.Label == itemData.Key.ToString("00"));
                    element.Value = itemData.Count();
                }

                return results.ToList();
            }

            private List<SimpleLabeledValue<int>> MostActiveUsers(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Username).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = "Anonymous";

                    if (itemData.Key != null)
                    {
                        name = itemData.Key.ToString();
                    }

                    results.Add(new SimpleLabeledValue<int>(name, itemData.Count()));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> MostFailedEndpoints(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "500" }, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, itemData.Count()));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> MostResourceHungryEndpoint(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, (int)itemData.Average(x => x.TotalTransactionTime)));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> BuildMostUsedEndpoints(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, null, null);

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, itemData.Count()));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> BuildSlowestReadEndpoints(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, new[] { "GET" });

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, (int)itemData.Average(x => x.Duration)));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> BuildSlowestWriteEndpoints(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, new[] { "POST", "PUT", "DELETE" });

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, (int)itemData.Average(x => x.Duration)));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> BuildTotalTimeComsumptionPerReadEndpoint(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, new[] { "GET" });

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, itemData.Sum(x => x.Duration)));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }

            private List<SimpleLabeledValue<int>> BuildTotalTimeComsumptionPerWriteEndpoint(List<AnalyticsEntry> data)
            {
                var results = new List<SimpleLabeledValue<int>>();

                var filteredData = PrefilterResults(data, new[] { "200", "204" }, new[] { "POST", "PUT", "DELETE" });

                var groupedData = filteredData.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, itemData.Sum(x => x.Duration)));
                }

                return results.OrderByDescending(x => x.Value).ToList();
            }
        }


    }
}
