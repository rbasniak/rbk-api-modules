using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public static class ChartJsExtensions
    {
        public static LinearChartBuilder CreateLinearChart(this List<NeutralDatePoint> data, GroupingType groupingType, bool appendExtraData = true)
        {
            return LinearChartBuilder.CreateLinearDateChart(data, groupingType, appendExtraData);
        }

        public static LinearChartBuilder CreateLinearChart(this List<NeutralCategoryPoint> data, bool appendExtraData = true)
        {
            return LinearChartBuilder.CreateLinearCategoryChart(data, appendExtraData);
        }

        public static RadialChartBuilder CreateRadialChart(this List<NeutralCategoryPoint> data, int maximumSeries, string mergedLabel, bool appendExtraData = true)
        {
            return RadialChartBuilder.CreateRadialCategoryChart(data, maximumSeries, mergedLabel, appendExtraData);
        }

        public static RadialChartBuilder CreateRadialChart(this List<NeutralCategoryPoint> data, bool appendExtraData = true)
        {
            return RadialChartBuilder.CreateRadialCategoryChart(data, Int32.MaxValue, "", appendExtraData);
        }
    }
}
