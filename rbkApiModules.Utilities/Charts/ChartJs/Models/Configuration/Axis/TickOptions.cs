using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TickOptions
    {
        public string BackdropColor { get; set; }
        public double? BackdropPaddingX { get; set; }
        public double? BackdropPaddingY { get; set; }
        public double? MaxTicksLimit { get; set; }
        public bool? ShowLabelBackdrop { get; set; }
    }
}
