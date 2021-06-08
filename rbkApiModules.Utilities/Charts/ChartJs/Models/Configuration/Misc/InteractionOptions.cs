using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class InteractionOptions
    {
        public bool? Intersect { get; set; }
        public IntersectMode? Mode { get; set; } 
        public AsixInteract? Axis { get; set; }
    }

    public enum IntersectMode
    {
        Nearest,
        Index,
        Dataset,
        Point,
        X,
        Y,
        Intersect
    }

    public enum AsixInteract
    {
        X,
        Y,
        XY
    }
}
