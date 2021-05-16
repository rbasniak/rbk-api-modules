using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LegendOptions
    {
        public bool? Display { get; set; }
        public string Position { get; set; }
        public bool? FullWdidth { get; set; }
        public bool? Reverse { get; set; }
        public LegendLabelOptions Labels { get; set; }

    }
}
