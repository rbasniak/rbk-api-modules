using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class ChartYAxe : CommonAxe
    {
        public ChartYAxe()
        {
            Type = ScaleType.Linear.ToString().ToLower();
        }
    }
}
