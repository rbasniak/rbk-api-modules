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

        public List<string> BackgroundColor { get; set; }
        public List<string> BorderColor { get; set; }
        public List<string> HoverBackgroundColor { get; set; }
        public List<string> HoverBorderColor { get; set; }

        public List<double> Data { get; set; }
        public List<List<object>> Extra { get; set; }
    }
}
