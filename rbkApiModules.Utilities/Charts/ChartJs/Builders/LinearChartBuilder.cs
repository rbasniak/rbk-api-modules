using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearChartBuilder : BaseChartBuilder<LinearChartBuilder, LinearChart>
    {

        public LinearChartBuilder(LinearChart chart) : base(chart)
        {

        }

        public static LinearChartBuilder CreateLinearDateChart(List<NeutralDatePoint> data, GroupingType groupingType, bool appendExtraData)
        {
            var fromDate = data.Min(x => x.Date);
            var toDate = data.Max(x => x.Date);

            var chart = new LinearChart();

            foreach (var serieData in data.GroupBy(x => x.SerieId))
            {
                var serie = new LinearDataset(serieData.First().SerieId);

                serie.Data = BuildLineChartAxis(fromDate, toDate, groupingType);

                foreach (var groupedSerieData in serieData.GroupBy(x => x.Date.GetGroupDate(groupingType)))
                {
                    var point = serie.Data.Single(x => x.X == new Point(groupedSerieData.First().Date, 0, groupingType, null).X);
                    point.Y = groupedSerieData.Sum(x => x.Value);

                    if (appendExtraData)
                    {
                        point.Data = groupedSerieData.SelectMany(x => x.Data).ToList();
                    }
                }

                chart.Data.Datasets.Add(serie);
            }

            return new LinearChartBuilder(chart);
        }

        public static LinearChartBuilder CreateLinearCategoryChart(List<NeutralCategoryPoint> data, bool appendExtraData)
        {
            var chart = new LinearChart();

            foreach (var serieData in data.GroupBy(x => x.SerieId))
            {
                var serie = new LinearDataset(serieData.First().SerieId);

                foreach (var groupedSerieData in serieData.GroupBy(x => x.Category))
                {
                    List<object> extraData = null;

                    if (appendExtraData)
                    {
                        extraData = groupedSerieData.SelectMany(x => x.Data).ToList();
                    }

                    serie.Data.Add(new Point(groupedSerieData.First().Category, groupedSerieData.Sum(x => x.Value), extraData));
                }

                chart.Data.Datasets.Add(serie);
            }

            return new LinearChartBuilder(chart);
        }

        public LinearDatasetBuilder<LinearChartBuilder, LinearChart> SetupDataset(string datasetId)
        {
            var dataset = Builder.Data.Datasets.FirstOrDefault(x => x.Id == datasetId);

            if (dataset == null) throw new ArgumentException($"Unknown dataset: {datasetId}");

            var orderedDatasets = Builder.Data.Datasets.Where(x => x.Order != null).ToList();

            if (orderedDatasets != null)
            {
                dataset.Order = orderedDatasets.Max(x => x.Order) + 1;
            }   
            else
            {
                dataset.Order = 0;
            }

            return new LinearDatasetBuilder<LinearChartBuilder, LinearChart>(this, dataset);
        }

        public LinearChartBuilder Theme(params ColorPallete[] palletes)
        {
            return Colors(ChartCollorSelector.GetColors(palletes), "ff");
        }

        public LinearChartBuilder Theme(string backgroundTransparency, params ColorPallete[] palletes)
        {
            return Colors(ChartCollorSelector.GetColors(palletes), backgroundTransparency);
        }

        public LinearChartBuilder Colors(string[] colors, string backgroundTransparency = "ff")
        {
            for (int i = 0; i < Builder.Data.Datasets.Count; i++)
            {
                var color = i < colors.Length ? colors[i] : "#777777";

                Builder.Data.Datasets[i].BackgroundColor = color.ToLower() + backgroundTransparency;
                Builder.Data.Datasets[i].BorderColor = color.ToLower();
            }

            return this;
        }

        public LinearChartBuilder WithXAxis(string axisId)
        {
            return WithAxis(axisId);
        }

        private LinearChartBuilder WithAxis(string axisId)
        {
            if (Builder.Config.Scales == null)
            {
                Builder.Config.Scales = new ExpandoObject();
            }

            var scales = Builder.Config.Scales as IDictionary<string, object>;

            var axis = new CartesianScale
            {
                Display = true
            };

            scales.Add(axisId, axis);

            return new CartesianScaleBuilder(this, axis);
        }







        public LinearChartBuilder SetXAxisBarPercentage(double value)
        {
            var axis = GetCurrentXAxis();

            axis.BarPercentage = value;

            return this;
        }

        public LinearChartBuilder HideXAxisGridlines()
        {
            var axis = GetCurrentXAxis();

            if (axis.GridLines == null)
            {
                axis.GridLines = new GridLineOptions();
            }

            axis.GridLines.Display = false;

            return this;
        }

        public LinearChartBuilder HideYAxisGridlines()
        {
            var axis = GetCurrentYAxis();

            if (axis.GridLines == null)
            {
                axis.GridLines = new GridLineOptions();
            }

            axis.GridLines.Display = false;

            return this;
        }

        

        public LinearChartBuilder SetYAxisPosition(PositionType position)
        {
            var axis = GetCurrentYAxis();

            axis.Position = position.ToString().ToLower();

            return this;
        }

        public LinearChartBuilder ShowYAxisLabel(string label)
        {
            var axis = GetCurrentYAxis();

            axis.ScaleLabel = new ScaleTitleOptions
            {
                Display = true,
                LabelString = label,
            };

            return this;
        }

        public LinearChartBuilder SetYAxisMinRange(double value)
        {
            var axis = GetCurrentYAxis();

            if (axis.Ticks == null)
            {
                axis.Ticks = new LinearTickOptions();
            }

            if (axis.Ticks is LinearTickOptions linearTicks)
            {
                linearTicks.Min = value;
            }
            else
            {
                throw new NotSupportedException("Minimum values are supported only on linear axis");
            }

            return this;
        }

        public LinearChartBuilder SetYAxisMaxRange(double value)
        { 
            var axis = GetCurrentXAxis();

            if (axis.Ticks == null)
            {
                axis.Ticks = new LinearTickOptions();
            }

            if (axis.Ticks is LinearTickOptions linearTicks)
            {
                linearTicks.Max = value;
            }
            else
            {
                throw new NotSupportedException("Maximum values are supported only on linear axis");
            }

            return this;
        }

        public LinearChartBuilder SetYAxisOverflow(AxisOverflowType type, AxisOverflowDirection direction, double value)
        {
            var axis = GetCurrentYAxis();

            return this;
        }

        private LinearDataset GetCurrentDataset()
        {
            if (_currentDataset != null)
            {
                return _currentDataset;
            }
            else
            {
                var dataset = Builder.Data.Datasets.FirstOrDefault();

                if (dataset == null) throw new NotSupportedException("The chart has no dataset to setup");

                return dataset;
            }
        } 

        private static List<Point> BuildLineChartAxis(DateTime startDate, DateTime endDate, GroupingType groupingType)
        {
            var date = startDate.GetGroupDate(groupingType);

            var axis = new List<Point>();

            while (date <= endDate.Date)
            {
                axis.Add(new Point(date.Date, 0, groupingType, null));

                switch (groupingType)
                {
                    case GroupingType.None:
                        throw new NotSupportedException();
                    case GroupingType.Hourly:
                        date = date.AddHours(1);
                        break;
                    case GroupingType.Daily:
                        date = date.AddDays(1);
                        break;
                    case GroupingType.Weekly:
                        date = date.AddDays(7);
                        break;
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

            if (axis.Last().X != new Point(endDate.Date, 0, groupingType, null).X)
            {
                axis.Add(new Point(endDate.Date, 0, groupingType, null));
            }

            return axis;
        }
    }
}
