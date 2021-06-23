using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public static class ChartJsExtensions
    {
        public static LinearChartBuilder CreateLinearChart(this IEnumerable<NeutralDatePoint> data, GroupingType groupingType, bool appendExtraData, DateTime? forceStartDate = null, DateTime? forceEndDate = null)
        {
            return LinearChartBuilder.CreateLinearDateChart(data.ToList(), groupingType, appendExtraData, forceStartDate, forceEndDate);
        }

        public static LinearChartBuilder CreateLinearChart(this IEnumerable<NeutralCategoryPoint> data, bool appendExtraData = true)
        {
            return LinearChartBuilder.CreateLinearCategoryChart(data.ToList(), appendExtraData);
        }

        public static RadialChartBuilder CreateRadialChart(this IEnumerable<NeutralCategoryPoint> data, int maximumSeries, string mergedLabel, bool appendExtraData = true)
        {
            return RadialChartBuilder.CreateRadialCategoryChart(data.ToList(), maximumSeries, mergedLabel, appendExtraData);
        }

        public static RadialChartBuilder CreateRadialChart(this IEnumerable<NeutralCategoryPoint> data, bool appendExtraData = true)
        {
            return RadialChartBuilder.CreateRadialCategoryChart(data.ToList(), Int32.MaxValue, "", appendExtraData);
        }
    }
}
