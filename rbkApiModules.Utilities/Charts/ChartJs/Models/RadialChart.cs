using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialChart: BaseChart
    {
        public RadialChart(): base()
        {
            Data = new RadialChartData();
        }

        public RadialChartData Data { get; set; }
    }
}
