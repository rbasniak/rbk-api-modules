using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public static class ChartJsExtensions
    {
        public static LinearChartBuilder<T> CreateLinearChart<T>(this IEnumerable<T> data)
        {
            return LinearChartBuilder<T>.CreateLinearDateChart(data.ToList());
        }  

        public static RadialChartBuilder<T> CreateRadialChart<T>(this IEnumerable<T> data)
        {
            return RadialChartBuilder<T>.CreateRadialCategoryChart(data);
        }
    }
}