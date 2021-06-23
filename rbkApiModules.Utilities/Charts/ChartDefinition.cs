using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts
{
    public class ChartDefinition
    {
        public ChartDefinition() 
        {
           
        }

        public string Id { get; set; }
        public ExpandoObject Chart { get; set; }
    }
}
