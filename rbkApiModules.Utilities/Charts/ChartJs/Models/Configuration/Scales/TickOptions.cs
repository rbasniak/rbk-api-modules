using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TickOptions
    {
        public bool? AutoSkip { get; internal set; }
        public string Color { get; internal set; }
        public bool? Display { get; internal set; }
        public double? Z { get; internal set; }
        public double? StepSize { get; internal set; }
        public double? Count { get; internal set; }
        public double? MaxTicksLimit { get; internal set; }
        public double? Precision { get; internal set; }
        public int? AutoSkipPadding { get; internal set; }
    }
}
