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

        public override LinearChartBuilder OfType(ChartType type)
        {
            if (type == ChartType.StackedBar)
            {
                WithAxis("x").Stacked();
                WithAxis("y").Stacked();
            }

            return base.OfType(type);
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

            if (orderedDatasets != null && orderedDatasets.Count > 0)
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
            return SetColors(ChartCollorSelector.GetColors(palletes), "ff");
        }

        public LinearChartBuilder Theme(string backgroundTransparency, params ColorPallete[] palletes)
        {
            return SetColors(ChartCollorSelector.GetColors(palletes), backgroundTransparency);
        }

        private LinearChartBuilder SetColors(string[] colors, string backgroundTransparency = "ff")
        {
            for (int i = 0; i < Builder.Data.Datasets.Count; i++)
            {
                var color = i < colors.Length ? colors[i] : "#777777";

                Builder.Data.Datasets[i].BackgroundColor = color.ToLower() + backgroundTransparency;
                Builder.Data.Datasets[i].BorderColor = color.ToLower();

                Builder.Data.Datasets[i].PointBackgroundColor = color.ToLower();
                Builder.Data.Datasets[i].PointBorderColor = color.ToLower();
            }

            return this;
        }

        public CartesianScaleBuilder<LinearChartBuilder, LinearChart> WithXAxis(string axisId)
        {
            return WithAxis(axisId);
        }

        public CartesianScaleBuilder<LinearChartBuilder, LinearChart> WithYAxis(string axisId)
        {
            return WithAxis(axisId);
        }

        private CartesianScaleBuilder<LinearChartBuilder, LinearChart> WithAxis(string axisId)
        {
            if (Builder.Config.Scales == null)
            {
                Builder.Config.Scales = new ExpandoObject();
            }

            var scales = Builder.Config.Scales as IDictionary<string, object>;

            if (!scales.TryGetValue(axisId, out object axis))
            {
                axis = new CartesianScale
                {
                    Display = true,
                };

                scales.Add(axisId, axis);
            }

            return new CartesianScaleBuilder<LinearChartBuilder, LinearChart>(this, (CartesianScale)axis);
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
