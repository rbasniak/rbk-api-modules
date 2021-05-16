using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class Scales
    {
        public ScaleType? Type { get; set; }
        public bool? Display { get; set; }
        public PositionType? Position { get; set; }
        public GridLineOptions GridLines { get; set; }
        public ScaleTitleOptions ScaleLabel { get; set; }
        public LinearTickOptions Ticks { get; set; }
        public List<ChartXAxe> XAxes { get; set; }
        public List<ChartYAxe> YAxes { get; set; }
    }
}
