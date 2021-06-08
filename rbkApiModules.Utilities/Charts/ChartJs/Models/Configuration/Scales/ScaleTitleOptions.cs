using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class ScaleTitleOptions
    {
        public string Color { get; set; }
        public bool? Display { get; set; }
        public FontOptions Font { get; set; }
        public PaddingOptions Padding { get; set; }
        public PositionType? Position { get; set; }
        public string Text { get; set; }
    }
}
