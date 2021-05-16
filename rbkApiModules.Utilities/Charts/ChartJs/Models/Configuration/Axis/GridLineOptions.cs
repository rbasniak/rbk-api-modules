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

        public bool? Display { get; set; }
        public string Color { get; set; }
        public double[] BorderDash { get; set; }
        public double? BorderDashOffset { get; set; }
        public double? LineWidth { get; set; }
        public bool? DrawBorder { get; set; }
        public bool? DrawOnChartArea { get; set; }
        public bool? DrawTicks { get; set; }
        public double? TickMarkLength { get; set; }
        public double? ZeroLineWidth { get; set; }
        public string ZeroLineColor { get; set; }
        public double[] ZeroLineBorderDash { get; set; }
        public double? ZeroLineBorderDashOffset { get; set; }
        public bool? OffsetGridLines { get; set; }
    }
}
