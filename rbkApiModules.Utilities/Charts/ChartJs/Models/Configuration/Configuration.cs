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
        public int? ResponsiveAnimationDuration { get; set; }

        public double? AspectRatio { get; set; }
        public bool MaintainAspectRatio { get; set; }
        public TitleOptions Title { get; set; }
        public LegendOptions Legend { get; set; }
        public TooltipOptions Tooltips { get; set; }
        public HoverOptions Hover { get; set; }
        public AnimationOptions Animation { get; set; }
        public ElementsOptions Options { get; set; }
        public LayoutOptions Layout { get; set; }
        public bool? ShowLines { get; set; }
        public bool? SpanGaps { get; set; }
        public double? CutoutPercentage { get; set; }
        public double? Circumference { get; set; }
        public Scales Scales { get; set; }
    }
}
