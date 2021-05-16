using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class ElementsOptions
    {
        public PointOptions Point { get; set; }
        public LineOptions Line { get; set; }
        public ArcOptions Arc { get; set; }
        public RectangleOptions Rectangle { get; set; }
    }
}
