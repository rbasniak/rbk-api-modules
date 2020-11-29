using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{ 
    public class ChartJsChartData<T>
    {
        public ChartJsChartData()
        {
            Labels = new List<string>();
            Datasets = new List<ChartJsDataset<T>>();
        }

        public List<string> Labels { get; set; }
        public List<ChartJsDataset<T>> Datasets { get; set; }
    }

    public class ChartJsDataset<T>
    {
        public ChartJsDataset()
        {
            Data = new List<ChartPoint<T>>();
        }

        public List<ChartPoint<T>> Data { get; set; }
        public string Label { get; set; }
        public double LineTension { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
    }

    public class ChartPoint<T>
    {
        public T X { get; set; }
        public double Y { get; set; }
        public object Data { get; set; }
    }
}
