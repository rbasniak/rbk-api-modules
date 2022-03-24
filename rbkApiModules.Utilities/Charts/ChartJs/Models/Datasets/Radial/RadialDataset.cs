using System;
using System.Collections.Generic;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialDataset : BaseDataset
    {
        public RadialDataset() : base(String.Empty)
        {
            Data = new List<double>();
            Extra = new List<List<object>>();
        }

        public double? Offset { get; internal set; }
        public List<string> BackgroundColor { get; internal set; }
        public List<string> BorderColor { get; internal set; }
        public List<string> HoverBackgroundColor { get; internal set; }
        public List<string> HoverBorderColor { get; internal set; }

        public double? BorderRadius { get; internal set; }
        public double? BorderWidth { get; internal set; }

        public double? Rotation { get; internal set; }

        public double? HoverBorderRadius { get; internal set; }
        public double? HoverBorderWidth { get; internal set; }
        public double? HoverOffset { get; internal set; }

        public double? Circumference { get; internal set; }

        public List<double> Data { get; internal set; }
        public List<List<object>> Extra { get; internal set; }
    }
}
