using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class CommonAxe
    {
        public string Type { get; set; }
        public bool? Display { get; set; }
        public string Id { get; set; }
        public bool? Stacked { get; set; }
        public string Position { get; set; }
        public TickOptions Ticks { get; set; }
        public GridLineOptions GridLines { get; set; }
        public int? BarThickness { get; set; }
        public ScaleTitleOptions ScaleLabel { get; set; }
    }
}
