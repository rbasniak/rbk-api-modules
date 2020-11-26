using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public class ChartJsData<T>
    {
        public ChartJsChartData<T> Data { get; set; }
        public ChartJsChartOptions Options { get; set; }
    }
}