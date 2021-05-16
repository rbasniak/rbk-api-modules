using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearDataset: BaseDataset
    {
        public LinearDataset(string id): base(id)
        {
            Data = new List<Point>();
        }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string HoverBackgroundColor { get; set; }
        public string HoverBorderColor { get; set; }

        public List<Point> Data { get; set; }
    }
}
