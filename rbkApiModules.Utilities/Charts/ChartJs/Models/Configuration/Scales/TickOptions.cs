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
        public string Color { get; set; }
        public bool? Display { get; set; }
        public double? Z { get; set; }
        public double? StepSize { get; set; }
        public double? Count { get; set; }
        public double? MaxTicksLimit { get; set; }
        public double? Precision { get; set; }
    }
}
