using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Utilities;
using rbkApiModules.Diagnostics.Core;

namespace rbkApiModules.Diagnostics
{
    internal static class ChartUtils
    {
        internal static async Task<List<LineChartSeries>> BuildChartData(IDiagnosticsModuleStore context, DateTime from, DateTime to, Func<DiagnosticsEntry, string> propertyFilter,
                Func<DiagnosticsEntry, string> nameFunction)
        {
            var results = new List<LineChartSeries>();

            var data = await context.FilterAsync(from, to, null, null, null, null, null, null, null, null, null, null, null, null);

            var groupedSeries = data.GroupBy(propertyFilter).ToList();

            foreach (var seriesData in groupedSeries)
            {
                var series = new LineChartSeries(nameFunction(seriesData.First()));

                foreach (var date in ChartingUtilities.BuildLineChartAxis(from, to))
                {
                    series.Data.Add(date);
                }

                var groupedData = seriesData.GroupBy(x => x.Timestamp.Date).ToList();

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var errors = itemData.Count();

                    var point = series.Data.First(x => x.Date == date);

                    point.Value = errors;
                }

                results.Add(series);
            }

            return results;
        }
    }
}
