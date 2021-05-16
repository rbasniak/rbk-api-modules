using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LineOptions
    {
        public double? Tension { get; set; }
        public string BackgroundColor { get; set; }
        public double? BorderWidth { get; set; }
        public string BorderColor { get; set; }
        public string BorderCapStyle { get; set; }
        public string[] BorderDash { get; set; }
        public double? BorderDashOffset { get; set; }
        public string BorderJoinStyle { get; set; }
        public bool? CapBezierPoints { get; set; }
        public bool? Fill { get; set; }
        public bool? Stepped { get; set; }
    }
}
