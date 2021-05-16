using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TimeScale
    {

        public TimeDisplayFormat DisplayFormats { get; set; }
        public bool? IsoWeekday { get; set; }
        public string Max { get; set; }
        public string Min { get; set; }
        public TimeUnit? Round { get; set; }
        public string TooltipFormat { get; set; }
        public TimeUnit? Unit { get; set; }
        public double? UnitStepSize { get; set; }
        public TimeUnit? MinUnit { get; set; }
    }
}
