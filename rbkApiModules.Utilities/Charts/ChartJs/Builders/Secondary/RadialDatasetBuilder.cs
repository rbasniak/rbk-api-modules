using System;
using System.Linq;

namespace rbkApiModules.Utilities.Charts.ChartJs
{ 
    public class RadialDatasetBuilder<TFactory, TChart>: DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public RadialDatasetBuilder(BaseChartBuilder<TFactory, TChart> builder) : base(builder)
        {
        }
    } 
}