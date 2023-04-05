using System.Collections.Generic;

namespace rbkApiModules.Commons.Charts.ChartJs
{
    public abstract class BaseChartData
    {
        public BaseChartData()
        {
            Labels = new List<string>();
        }
        public List<string> Labels { get; internal set; }
    } 
}
