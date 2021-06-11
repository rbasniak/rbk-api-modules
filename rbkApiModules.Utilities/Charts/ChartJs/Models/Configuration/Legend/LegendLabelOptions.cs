using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LegendLabelOptions
    {
        public double? BoxHeight { get; internal set; }
        public double? BoxWidth { get; internal set; }
        public string? Color { get; internal set; }
        public FontOptions Font { get; internal set; }
        public double? Padding { get; internal set; }
        public bool? UsePointStyle { get; internal set; }
    }
}
