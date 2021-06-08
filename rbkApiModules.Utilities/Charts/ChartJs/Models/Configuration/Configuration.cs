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
            MaintainAspectRatio = false;
        }

        public bool Responsive { get; set; }
        public bool MaintainAspectRatio { get; set; }
        public double? AspectRatio { get; set; }
        public LayoutOptions Layout { get; set; }
        public InteractionOptions Interaction { get; set; }
        public dynamic Scales { get; set; }
        public PluginOptions Plugins { get; set; }
    }
}
