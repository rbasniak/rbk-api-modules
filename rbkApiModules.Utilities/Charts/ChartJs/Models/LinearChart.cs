using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearChart : BaseChart
    {
        public LinearChart() : base()
        {
            Data = new LinearChartData();
        }

        public LinearChartData Data { get; set; }
    }
}
