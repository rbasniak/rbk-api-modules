using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class PluginOptions
    {
        public TitleOptions Title { get; set; }
        public TooltipOptions Tooltip { get; set; }
        public LegendOptions Legend { get; set; }
    }
}
