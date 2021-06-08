using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearChartData: BaseChartData
    {
        public LinearChartData(): base()
        {
            Datasets = new List<LinearDataset>();
        }
        public List<LinearDataset> Datasets { get; set; }
    }
}
