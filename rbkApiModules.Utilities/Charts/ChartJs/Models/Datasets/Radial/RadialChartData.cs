using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialChartData: BaseChartData
    {
        public RadialChartData(): base()
        {
            Datasets = new List<RadialDataset>();
        }
        public List<RadialDataset> Datasets { get; set; }
    }
}
