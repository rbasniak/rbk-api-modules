using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class BaseChartData
    {
        public BaseChartData()
        {
            Labels = new List<string>();
        }
        public List<string> Labels { get; internal set; }
    } 
}
