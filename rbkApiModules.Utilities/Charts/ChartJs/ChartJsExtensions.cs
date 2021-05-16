using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public static class ChartJsExtensions
    {
        public static LinearChartFactory CreateLinearChart(this List<NeutralDatePoint> data, GroupingType groupingType, bool appendExtraData = true)
        {
            return LinearChartFactory.CreateLinearDateChart(data, groupingType, appendExtraData);

        }

        public static LinearChartFactory CreateLinearChart(this List<NeutralCategoryPoint> data, bool appendExtraData = true)
        {
            return LinearChartFactory.CreateLinearCategoryChart(data, appendExtraData);
        }

        public static RadialChartFactory CreateRadialChart(this List<NeutralCategoryPoint> data, int maximumSeries, string mergedLabel, bool appendExtraData = true)
        {
            return RadialChartFactory.CreateRadialCategoryChart(data, maximumSeries, mergedLabel, appendExtraData);
        }

        public static RadialChartFactory CreateRadialChart(this List<NeutralCategoryPoint> data, bool appendExtraData = true)
        {
            return RadialChartFactory.CreateRadialCategoryChart(data, Int32.MaxValue, "", appendExtraData);
        }
    }
}
