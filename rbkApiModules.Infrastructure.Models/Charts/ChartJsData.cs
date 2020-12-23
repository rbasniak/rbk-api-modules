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
        private int _colorIndex = 0;

        public ChartJsData(ColorPallete pallete)
        {
            Data = new ChartJsChartData<T>();
            Options = new ChartJsChartOptions();
            ColorPallete = pallete;
        }

        public ChartJsData(ColorPallete pallete, string title) : this(pallete)
        {
            Options.Title.Text = title;
        } 

        public ChartJsData(ColorPallete pallete, string id, string title): this(pallete, title)
        {
            Id = id;
        }

        public string Id { get; set; }
        public ColorPallete ColorPallete { get; set; }
        public ChartJsChartData<T> Data { get; set; }
        public ChartJsChartOptions Options { get; set; }

        public ChartJsDataset<T> AddSeries(string name, DateTime startDate, DateTime endDate, GroupingType groupingType, string transparency = "ff", string color = null, double lineTension = 0)
        {
            var dataset = new ChartJsDataset<T>();
            var axisData = BuildLineChartAxis(startDate, endDate, groupingType);

            dataset.Label = name;
            dataset.LineTension = lineTension;
            
            if (String.IsNullOrEmpty(color))
            {
                color = ChartCollorPicker.NextColor(ColorPallete, _colorIndex++);
            }
            
            dataset.BackgroundColor = color + transparency;
            dataset.BorderColor = color;

            foreach (var item in axisData)
            {
                dataset.Data.Add(new ChartPoint<T>((T)((object)item.X), 0, null));
            }

            Data.Datasets.Add(dataset);

            switch (groupingType)
            {
                case GroupingType.None:
                    break;
                case GroupingType.Daily:
                    Data.Labels = dataset.Data.Select(x => ((DateTime)((object)x.X)).ToString(@"dd/MMM", new CultureInfo("PT-br"))).ToList();
                    break;
                case GroupingType.Hourly:
                    break;
                case GroupingType.Weekly:
                    Data.Labels = dataset.Data.Select(x => ((DateTime)((object)x.X)).ToString(@"dd/MMM", new CultureInfo("PT-br"))).ToList();
                    break;
                case GroupingType.Monthly:
                    Data.Labels = dataset.Data.Select(x => ((DateTime)((object)x.X)).ToString(@"MMM/yy", new CultureInfo("PT-br"))).ToList();
                    break;
                case GroupingType.Yearly:
                    Data.Labels = dataset.Data.Select(x => ((DateTime)((object)x.X)).ToString(@"yyyy", new CultureInfo("PT-br"))).ToList();
                    break;
                default:
                    break;
            }

            return dataset;
        }

        private List<ChartPoint<DateTime>> BuildLineChartAxis(DateTime startDate, DateTime endDate, GroupingType groupingType)
        {
            DateTime date;

            switch (groupingType)
            {
                case GroupingType.None:
                    throw new NotSupportedException();
                case GroupingType.Hourly:
                    throw new NotImplementedException();
                case GroupingType.Daily:
                    date = startDate;
                    break;
                case GroupingType.Weekly:
                    throw new NotImplementedException();
                case GroupingType.Monthly:
                    date = new DateTime(startDate.Year, startDate.Month, 1);
                    break;
                case GroupingType.Yearly:
                    date = new DateTime(startDate.Year, 1, 1);
                    break;
                default:
                    throw new NotSupportedException();
            } 

            var axis = new List<ChartPoint<DateTime>>();

            while (date <= endDate.Date)
            {
                axis.Add(new ChartPoint<DateTime>(date.Date, 0));

                switch (groupingType)
                {
                    case GroupingType.None:
                        throw new NotSupportedException();
                    case GroupingType.Hourly:
                        throw new NotImplementedException();
                    case GroupingType.Daily:
                        date = date.AddDays(1);
                        break;
                    case GroupingType.Weekly:
                        throw new NotImplementedException();
                    case GroupingType.Monthly:
                        date = date.AddMonths(1);
                        break;
                    case GroupingType.Yearly:
                        date = date.AddYears(1);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (axis.Last().X.Date != endDate.Date)
            {
                axis.Add(new ChartPoint<DateTime>(endDate.Date, 0));
            }

            return axis;
        }
    }
}