using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs
{
    public class DateTimeChart: BaseChart
    {
        public DateTimeChart(ColorPallete pallete): base(pallete)
        {
            Data = new DateTimeChartData();
            Options = new ChartOptions();
            ColorPallete = pallete;
        }

        public DateTimeChart(ColorPallete pallete, string title) : base(pallete, title)
        {
            Data = new DateTimeChartData();
        } 

        public DateTimeChart(ColorPallete pallete, string id, string title): base(pallete, id, title)
        {
            Data = new DateTimeChartData();
        }

        public DateTimeChartData Data { get; set; }

        public DateTimeDataSet AddSeries(string name, DateTime startDate, DateTime endDate, GroupingType groupingType, string transparency = "ff", string color = null, double lineTension = 0)
        {
            var dataset = new DateTimeDataSet();
            var axisData = BuildLineChartAxis(startDate, endDate, groupingType);

            dataset.Label = name;
            dataset.LineTension = lineTension;
            
            if (String.IsNullOrEmpty(color))
            {
                color = ChartCollorSelector.NextColor(ColorPallete, _colorIndex++);
            }
            
            dataset.BackgroundColor = color + transparency;
            dataset.BorderColor = color;

            foreach (var item in axisData)
            {
                dataset.Data.Add(new DateTimePoint(item.X, 0, null));
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

        private List<DateTimePoint> BuildLineChartAxis(DateTime startDate, DateTime endDate, GroupingType groupingType)
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

            var axis = new List<DateTimePoint>();

            while (date <= endDate.Date)
            {
                axis.Add(new DateTimePoint(date.Date, 0));

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
                axis.Add(new DateTimePoint(endDate.Date, 0));
            }

            return axis;
        }
    }
}