using System.Collections.Generic;

namespace rbkApiModules.Commons.Charts.ChartJs
{
    public class RadialChartData: BaseChartData
    {
        public RadialChartData(): base()
        {
            Datasets = new List<RadialDataset>();
        }
        public List<RadialDataset> Datasets { get; internal set; }
    }
}
