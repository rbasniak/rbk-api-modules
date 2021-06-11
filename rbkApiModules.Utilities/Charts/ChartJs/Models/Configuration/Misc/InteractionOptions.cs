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
        public bool? Intersect { get; internal set; }
        public string Mode { get; internal set; } 
        public string Axis { get; internal set; }

        internal void SetAxis(AsixInteract axis)
        {
            Axis = axis.ToString().ToLower();
        }

        internal void SetIntersectMode(IntersectMode mode)
        {
            Mode = mode.ToString().ToLower();
        }
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
