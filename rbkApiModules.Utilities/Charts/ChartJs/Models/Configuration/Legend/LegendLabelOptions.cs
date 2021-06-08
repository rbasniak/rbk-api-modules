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
        public double? BoxHeight { get; set; }
        public double? BoxWidth { get; set; }
        public string? Color { get; set; }
        public FontOptions Font { get; set; }
        public double? Padding { get; set; }
        public bool? UsePointStyle { get; set; }
    }
}
