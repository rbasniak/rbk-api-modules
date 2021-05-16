using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TitleOptions
    {
        public bool? Display { get; set; }
        public PositionType? Position { get; set; }
        public bool? FullWdith { get; set; }
        public double? FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string FontStyle { get; set; }
        public double? Padding { get; set; }
        public string Text { get; set; }
    }
}
