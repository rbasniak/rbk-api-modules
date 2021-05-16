using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs.Deprecated
{ 
    public abstract class BaseChartData
    {
        public BaseChartData()
        {
            Labels = new List<string>();
        }

        public List<string> Labels { get; set; }
    }





    public class DateTimeChartData: BaseChartData
    {
        public DateTimeChartData(): base()
        {
            Labels = new List<string>();
            Datasets = new List<DateTimeDataSet>();
        }

        public List<DateTimeDataSet> Datasets { get; set; }
    }

    public class DateTimeDataSet
    {
        public DateTimeDataSet()
        {
            Data = new List<DateTimePoint>();
        }

        public List<DateTimePoint> Data { get; set; }
        public string Label { get; set; }
        public double LineTension { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
    }

    public class DateTimePoint
    {
        public DateTimePoint(DateTime x, double y, object data = null)
        {
            X = x;
            Y = y;
            Data = data;
        }

        public DateTime X { get; set; }
        public double Y { get; set; }
        public object Data { get; set; }
    }





    public class CategoryChartData : BaseChartData
    {
        public CategoryChartData() : base()
        {
            Labels = new List<string>();
            Datasets = new List<CategoryDataSet>();
        }

        public List<CategoryDataSet> Datasets { get; set; }
    }

    public class CategoryDataSet
    {
        public CategoryDataSet()
        {
            Data = new List<double>();
            BorderColor = new List<string>();
            BackgroundColor = new List<string>();
            BorderWidth = 2;
        }

        public List<double> Data { get; set; }
        public string Label { get; set; }
        public List<string> BorderColor { get; set; }
        public List<string> BackgroundColor { get; set; }
        public int BorderWidth { get; set; }
    }
}
