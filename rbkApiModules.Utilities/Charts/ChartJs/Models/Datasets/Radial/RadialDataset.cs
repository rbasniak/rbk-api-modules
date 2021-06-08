using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialDataset : BaseDataset
    {
        public RadialDataset() : base(String.Empty)
        {
            Data = new List<double>();
            Extra = new List<List<object>>();
        }

        public double? Offset { get; set; }
        public List<string> BackgroundColor { get; set; }
        public List<string> BorderColor { get; set; }
        public List<string> HoverBackgroundColor { get; set; }
        public List<string> HoverBorderColor { get; set; }

        public double? BorderRadius { get; set; }
        public double? BorderWidth { get; set; }

        public double? Rotation { get; set; }

        public double? HoverBorderRadius { get; set; }
        public double? HoverBorderWidth { get; set; }
        public double? HoverOffset { get; set; }

        public double? Circumference { get; set; }

        public List<double> Data { get; set; }
        public List<List<object>> Extra { get; set; }
    }
}
