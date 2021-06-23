using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class Point
    {
        public Point(string category, double value, object data = null)
        {
            X = category;
            Y = value;
            Data = data;
        }

        public Point(DateTime date, double value, GroupingType groupingType, object data = null)
        {
            switch (groupingType)
            {
                case GroupingType.None:
                    throw new NotSupportedException("Grouping type None is not supported and should not be used");
                case GroupingType.Hourly:
                    X = $"{date.Day}/{GetMonthName(date)}/{date.Year} {date.Hour}h";
                    break;
                case GroupingType.Weekly:
                case GroupingType.Daily:
                    X = $"{date.Day}/{GetMonthName(date)}/{date.Year}";
                    break;
                case GroupingType.Monthly:
                    X = $"{GetMonthName(date)}/{date.Year}";
                    break;
                case GroupingType.Yearly:
                    X = date.Year.ToString();
                    break;
                default:
                    throw new NotSupportedException($"Unknown grouping type: {groupingType}");
            }

            Y = value;
            Data = data;
        }

        public string X { get; set; }
        public double Y { get; set; }
        public object Data { get; set; }

        internal void RoundValue(int decimals)
        {
            Y = Math.Round(Y, decimals);
        }

        private string GetMonthName(DateTime date)
        {
            var culture = new CultureInfo("pt-BR", false);

            return culture.DateTimeFormat.GetMonthName(date.Month).ToUpper().Substring(0, 3);
        }
    }
}
