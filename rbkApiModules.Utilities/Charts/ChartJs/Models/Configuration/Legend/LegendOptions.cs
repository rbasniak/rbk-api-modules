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
        public AlignmentType? Align { get; set; }
        public bool? Display { get; set; }
        public bool? FullWdidth { get; set; }
        public LegendLabelOptions Labels { get; set; }
        public PositionType? Position { get; set; }
        public bool? Reverse { get; set; }
        public LegendTitleOptions Title { get; set; }
    }
}
