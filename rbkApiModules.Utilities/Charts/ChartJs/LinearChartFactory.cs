using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearChartFactory : BaseChartFactory<LinearChartFactory, LinearChart>
    {
        private LinearDataset _currentDataset;
        private ChartYAxe _currentYAxis;
        private ChartXAxe _currentXAxis;

        public LinearChartFactory(LinearChart chart) : base(chart)
        {

        }

        public static LinearChartFactory CreateLinearDateChart(List<NeutralDatePoint> data, GroupingType groupingType, bool appendExtraData)
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

            return new LinearChartFactory(chart);
        }

        public static LinearChartFactory CreateLinearCategoryChart(List<NeutralCategoryPoint> data, bool appendExtraData)
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

            return new LinearChartFactory(chart);
        }

        public LinearChartFactory SetCurrentDataset(string datasetId)
        {
            var dataset = _chart.Data.Datasets.FirstOrDefault(x => x.Id == datasetId);

            if (dataset == null) throw new ArgumentException($"Unknown dataset: {datasetId}");

            _currentDataset = dataset;

            return this;
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

        public LinearChartFactory SetDatasetType(ChartType type)
        {
            var dataset = GetCurrentDataset();

            dataset.Type = type.ToString().Substring(0, 1).ToLower() + type.ToString().Substring(1);

            if (type == ChartType.Line)
            {
                dataset.LineTension = 0;
                dataset.Fill = false;
            }
            else if (type == ChartType.Bar)
            {
                dataset.Fill = true;
            }

            return this;
        }

        public LinearChartFactory SetDataSetOrder(params string[] datasets)
        {
            var orderedDatasets = new List<LinearDataset>();

            if (datasets.Length != _chart.Data.Datasets.Count) throw new ArgumentException("The number of names must be the same of the number of datasets in the chart");

            foreach (var datasetName in datasets)
            {
                var dataset = _chart.Data.Datasets.FirstOrDefault(x => x.Id == datasetName);

                if (dataset == null) throw new ArgumentException($"Dataset not found: {datasetName}");

                orderedDatasets.Add(dataset);
            }

            _chart.Data.Datasets = orderedDatasets;

            return this;
        }

        public LinearChartFactory UseStackedBars()
        {
            var xAxis = GetCurrentXAxis();
            var yAxis = GetCurrentYAxis();

            xAxis.Stacked = true;
            yAxis.Stacked = true;

            return this;
        }

        public LinearChartFactory SetDatasetValuesRounding(int decimals)
        {
            var dataset = GetCurrentDataset();

            foreach (var item in dataset.Data)
            {
                item.RoundValue(decimals);
            }

            return this;
        }

        public LinearChartFactory SetDatasetLabel(string label)
        {
            var dataset = GetCurrentDataset();

            dataset.Label = label;

            return this;
        }

        public LinearChartFactory SetDatasetBorderWidth(double thickness)
        {
            var dataset = GetCurrentDataset();

            dataset.BorderWidth = thickness;

            return this;
        }

        public LinearChartFactory SetDatasetYAxis(string axisId)
        {
            var dataset = GetCurrentDataset();

            dataset.yAxisID = axisId;

            return this;
        }

        public LinearChartFactory SetColors(params ColorPallete[] palletes)
        {
            return SetColors(ChartCollorSelector.GetColors(palletes), "ff");
        }

        public LinearChartFactory SetColors(string backgroundTransparency, params ColorPallete[] palletes)
        {
            return SetColors(ChartCollorSelector.GetColors(palletes), backgroundTransparency);
        }

        public LinearChartFactory SetColors(string[] colors, string backgroundTransparency = "ff")
        {
            for (int i = 0; i < _chart.Data.Datasets.Count; i++)
            {
                var color = i < colors.Length ? colors[i] : "#777777";

                _chart.Data.Datasets[i].BackgroundColor = color.ToLower() + backgroundTransparency;
                _chart.Data.Datasets[i].BorderColor = color.ToLower();
            }

            return this;
        }

        public LinearChartFactory AddXAxis(string axisId)
        {
            if (_chart.Config.Scales == null)
            {
                _chart.Config.Scales = new Scales();
            }

            if (_chart.Config.Scales.XAxes == null)
            {
                _chart.Config.Scales.XAxes = new List<ChartXAxe>();
            }

            var axis = new ChartXAxe
            {
                Display = true,
                Id = axisId
            };

            _chart.Config.Scales.XAxes.Add(axis);

            _currentXAxis = axis;

            return this;
        }

        public LinearChartFactory SetXAxisBarPercentage(double value)
        {
            var axis = GetCurrentXAxis();

            axis.BarPercentage = value;

            return this;
        }

        public LinearChartFactory HideXAxisGridlines()
        {
            var axis = GetCurrentXAxis();

            if (axis.GridLines == null)
            {
                axis.GridLines = new GridLineOptions();
            }

            axis.GridLines.Display = false;

            return this;
        }

        public LinearChartFactory HideYAxisGridlines()
        {
            var axis = GetCurrentYAxis();

            if (axis.GridLines == null)
            {
                axis.GridLines = new GridLineOptions();
            }

            axis.GridLines.Display = false;

            return this;
        }

        public LinearChartFactory AddYAxis(string axisId)
        {
            if (_chart.Config.Scales == null)
            {
                _chart.Config.Scales = new Scales();
            }

            if (_chart.Config.Scales.YAxes == null)
            {
                _chart.Config.Scales.YAxes = new List<ChartYAxe>();
            }

            var axe = new ChartYAxe
            {
                Display = true,
                Id = axisId
            };

            _chart.Config.Scales.YAxes.Add(axe);

            _currentYAxis = axe;

            return this;
        }

        public LinearChartFactory SetYAxisPosition(PositionType position)
        {
            var axis = GetCurrentYAxis();

            axis.Position = position.ToString().ToLower();

            return this;
        }

        public LinearChartFactory ShowYAxisLabel(string label)
        {
            var axis = GetCurrentYAxis();

            axis.ScaleLabel = new ScaleTitleOptions
            {
                Display = true,
                LabelString = label,
            };

            return this;
        }

        public LinearChartFactory SetYAxisMinRange(double value)
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

        public LinearChartFactory SetYAxisMaxRange(double value)
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

        public LinearChartFactory SetYAxisOverflow(AxisOverflowType type, AxisOverflowDirection direction, double value)
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
                var dataset = _chart.Data.Datasets.FirstOrDefault();

                if (dataset == null) throw new NotSupportedException("The chart has no dataset to setup");

                return dataset;
            }
        }

        private ChartYAxe GetCurrentYAxis()
        {
            if (_currentYAxis == null)
            {
                if (_chart.Config.Scales == null)
                {
                    _chart.Config.Scales = new Scales();
                }

                if (_chart.Config.Scales.YAxes == null)
                {
                    _chart.Config.Scales.YAxes = new List<ChartYAxe>();

                    var axis = new ChartYAxe
                    {
                        Display = true,
                    };

                    _chart.Config.Scales.YAxes.Add(axis);
                }

                _currentYAxis = _chart.Config.Scales.YAxes.First();
            }

            return _currentYAxis;
        }

        private ChartXAxe GetCurrentXAxis()
        {
            if (_currentXAxis == null)
            {
                if (_chart.Config.Scales == null)
                {
                    _chart.Config.Scales = new Scales();
                }

                if (_chart.Config.Scales.XAxes == null)
                {
                    _chart.Config.Scales.XAxes = new List<ChartXAxe>();

                    var axis = new ChartXAxe
                    {
                        Display = true,
                    };

                    _chart.Config.Scales.XAxes.Add(axis);
                }

                _currentXAxis = _chart.Config.Scales.XAxes.First();
            }

            return _currentXAxis;
        }
    }
}
