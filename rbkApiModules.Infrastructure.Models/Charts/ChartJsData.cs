using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public class ChartJsData<T>
    {
        public ChartJsData()
        {
            Data = new ChartJsChartData<T>();
            Options = new ChartJsChartOptions();
        }

        public ChartJsData(string title) : this()
        {
            Options.Title.Text = title;
        } 

        public ChartJsData(string id, string title): this(title)
        {
            Id = id;
        }

        public string Id { get; set; }
        public ChartJsChartData<T> Data { get; set; }
        public ChartJsChartOptions Options { get; set; }

        public ChartJsDataset<T> AddSeries(string name, DateTime startDate, DateTime endDate, string borderColor, string backgroundColor, double lineTension = 0)
        {
            var dataset = new ChartJsDataset<T>();
            var axisData = BuildLineChartAxis(startDate, endDate);

            dataset.Label = name;
            dataset.LineTension = lineTension;
            dataset.BackgroundColor = backgroundColor;
            dataset.BorderColor = borderColor;

            foreach (var item in axisData)
            {
                dataset.Data.Add(new ChartPoint<T>((T)((object)item.X), 0, null));
            }

            Data.Datasets.Add(dataset);

            Data.Labels = dataset.Data.Select(x => ((DateTime)((object)x.X)).ToString(@"dd/MMM", new CultureInfo("PT-br"))).ToList();

            return dataset;
        }

        private List<ChartPoint<DateTime>> BuildLineChartAxis(DateTime startDate, DateTime endDate)
        {
            var date = startDate;

            var axis = new List<ChartPoint<DateTime>>();

            while (date <= endDate.Date)
            {
                axis.Add(new ChartPoint<DateTime>(date.Date, 0));
                date = date.AddDays(1);
            }

            if (axis.Last().X.Date != endDate.Date)
            {
                axis.Add(new ChartPoint<DateTime>(endDate.Date, 0));
            }

            return axis;
        }
    }
}