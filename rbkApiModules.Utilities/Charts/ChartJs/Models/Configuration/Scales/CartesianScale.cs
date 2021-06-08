using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class CartesianScale
    {
        public IndexAxis Axis { get; set; }
        public bool? Display { get; set; }
        public GridLineOptions? Grid { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public bool? Offset { get; set; }
        public AxisPosition Position { get; set; }
        public bool? Reverse { get; set; }
        public bool? Stacked { get; set; } 
        public TickOptions Ticks { get; set; }
        public ScaleTitleOptions Title { get; set; }

        public bool? BeginAtZero { get; set; }
        public double? SuggestedMax { get; set; }
        public double? SuggestedMin { get; set; }

    }
}
