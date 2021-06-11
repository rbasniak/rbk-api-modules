using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class Configuration
    {
        public Configuration()
        {
            Responsive = true;
            Plugins = new PluginOptions();
        }

        public bool? Responsive { get; internal set; }
        public bool? MaintainAspectRatio { get; internal set; }
        public double? AspectRatio { get; internal set; }
        public LayoutOptions Layout { get; internal set; }
        public InteractionOptions Interaction { get; internal set; }
        public dynamic Scales { get; internal set; }
        public PluginOptions Plugins { get; internal set; }
        public string IndexAxis { get; internal set; }
    }
}
