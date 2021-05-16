using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class PointOptions
    {
        public int? Radius { get; set; }
        public PointStyle? PointStyle { get; set; }
        public string BackgroundColor { get; set; }
        public double? BorderWidth { get; set; }
        public string BorderColor { get; set; }
        public double? HitRadius { get; set; }
        public double? HoverRadius { get; set; }
        public double? HoverBorderWidth { get; set; }
    }

}
