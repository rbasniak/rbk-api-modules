using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class ChartXAxe : CommonAxe
    {
        public double? CategoryPercentage { get; set; }
        public double? BarPercentage { get; set; }
        public TimeScale Time { get; set; }
    }
}
