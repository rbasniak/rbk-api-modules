using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class BaseChart
    {
        public BaseChart()
        {
            Config = new Configuration();
        }

        public string Type { get; set; }
        public Configuration Config { get; set; }
    }
}
