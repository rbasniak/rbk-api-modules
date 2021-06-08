using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class GridLineOptions
    {
        public string BorderColor { get; set; }
        public double[] BorderDash { get; set; }
        public double? BorderDashOffset { get; set; }
        public double? BorderWidth { get; set; }
        public string Color { get; set; }
        public bool? Display { get; set; }
        public bool? DrawBorder { get; set; }
        public bool? DrawOnChartArea { get; set; }
        public bool? DrawTicks { get; set; }
        public double? LineWidth { get; set; }
        public bool? Offset { get; set; }
        public double[] TickBorderDash { get; set; }
        public double? TickBorderDashOffset { get; set; }
        public string TickColor { get; set; }
        public double? TickLength { get; set; }
        public double? TickWidth { get; set; }
    }
}
