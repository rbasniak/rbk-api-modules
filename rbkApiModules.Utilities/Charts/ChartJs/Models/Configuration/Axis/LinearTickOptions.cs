using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearTickOptions : TickOptions
    {

        public bool? BeginAtZero { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? MaxTicksLimit { get; set; }
        public double? StepSize { get; set; }
        public double? SuggestedMin { get; set; }
        public double? SuggestedMax { get; set; }
    }
}
