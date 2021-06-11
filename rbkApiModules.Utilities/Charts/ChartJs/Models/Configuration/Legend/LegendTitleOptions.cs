using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LegendTitleOptions
    {
        public string Color { get; internal set; }
        public bool? Display { get; internal set; }
        public FontOptions Font { get; internal set; }
        public PaddingOptions Padding { get; internal set; }
        public string Position { get; internal set; }
        public string Text { get; internal set; }

        internal void SetAlignment(AlignmentType alignment)
        {
            Position = alignment.ToString().ToLower();
        }
    }
}
