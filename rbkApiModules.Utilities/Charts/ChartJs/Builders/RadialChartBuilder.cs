using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialChartBuilder : BaseChartBuilder<RadialChartBuilder, RadialChart>
    {
        public RadialChartBuilder(RadialChart chart) : base(chart)
        {

        }

        public static RadialChartBuilder CreateRadialCategoryChart(List<NeutralCategoryPoint> data, bool appendExtraData)
        {
            return CreateRadialCategoryChart(data, Int32.MaxValue, String.Empty, appendExtraData);
        }

        public static RadialChartBuilder CreateRadialCategoryChart(List<NeutralCategoryPoint> data, int maximumSeries, string mergedLabel, bool appendExtraData)
        {
            var chart = new RadialChart();

            if (data.GroupBy(x => x.SerieId).Count() > 1) throw new NotSupportedException("Radial charts can have only one dataset");

            var serie = new RadialDataset();
            var groupedData = data.GroupBy(x => x.Category).OrderByDescending(x => x.Sum(v => v.Value)).ToList();

            var untouchedData = new List<IGrouping<string, NeutralCategoryPoint>>();
            var mergedData = new List<IGrouping<string, NeutralCategoryPoint>>();

            if (groupedData.Count == maximumSeries)
            {
                maximumSeries = Int32.MaxValue;
            }

            for (int i = 0; i < maximumSeries - 1; i++)
            {
                if (i >= groupedData.Count) break;

                untouchedData.Add(groupedData[i]);
            }

            for (int i = maximumSeries - 1; i < groupedData.Count; i++)
            {
                mergedData.Add(groupedData[i]);
            }

            foreach (var serieData in untouchedData)
            {
                serie.Data.Add(serieData.Sum(x => x.Value));
                if (appendExtraData)
                {
                    serie.Extra.Add(serieData.SelectMany(x => x.Data).ToList());
                }
                chart.Data.Labels.Add(serieData.Key);
            }

            if (mergedData.Count > 0)
            {
                var allMergedData = mergedData.SelectMany(x => x);
                serie.Data.Add(allMergedData.Sum(x => x.Value));

                if (appendExtraData)
                {
                    serie.Extra.Add(allMergedData.SelectMany(x => x.Data).ToList());
                }

                chart.Data.Labels.Add(mergedLabel);
            }

            chart.Data.Datasets.Add(serie);

            return new RadialChartBuilder(chart);
        }

        public RadialChartBuilder SetValuesRounding(int decimals)
        {
            var dataset = Builder.Data.Datasets.SingleOrDefault();

            if (dataset == null) throw new NotSupportedException($"Radial charts can have only one dataset");

            for (int i = 0; i < dataset.Data.Count; i++)
            {
                dataset.Data[i] = Math.Round(dataset.Data[i], decimals);
            }

            return this;
        }

        public RadialChartBuilder Theme(params ColorPallete[] palletes)
        {
            return Colors(ChartCollorSelector.GetColors(palletes), "ff");
        }

        public RadialChartBuilder Theme(string backgroundTransparency, params ColorPallete[] palletes)
        {
            return Colors(ChartCollorSelector.GetColors(palletes), backgroundTransparency);
        }

        public RadialChartBuilder Colors(string[] colors, string backgroundTransparency = "ff")
        {
            if (colors == null || colors.Length == 0) throw new ArgumentException("You ness to speficify at least one color");

            var dataset = Builder.Data.Datasets.SingleOrDefault();

            if (dataset == null) throw new NotSupportedException($"Radial charts can have only one dataset");

            dataset.BackgroundColor = new List<string>();
            dataset.HoverBackgroundColor = new List<string>();

            for (int i = 0; i < dataset.Data.Count; i++)
            {
                var color = i < colors.Length ? colors[i] : "#777777";

                dataset.BackgroundColor.Add(color);
                dataset.HoverBackgroundColor.Add(color + backgroundTransparency);
            }

            return this;
        }

        public RadialChartBuilder RoundToNearestStorageUnit()
        { 
            var dataset = Builder.Data.Datasets.First();

            var averageSize = dataset.Data.Average();
            var unit = String.Empty;

            for (int i = 0; i < dataset.Data.Count; i++)
            {
                if (averageSize < 1024)
                {
                    unit = "bytes";
                }
                else if (averageSize < 1048576.0)
                {
                    unit = "kb";
                    dataset.Data[i] = Math.Round(dataset.Data[i] / 1024.0, 1);
                }
                else if (averageSize < 1073741824.0)
                {
                    unit = "mb";
                    dataset.Data[i] = Math.Round(dataset.Data[i] / 1048576.0, 1);
                }
                else if (averageSize < 1099511627776.0)
                {
                    unit = "gb";
                    dataset.Data[i] = Math.Round(dataset.Data[i] / 1073741824.0, 1);
                }
                else if (averageSize < 1125899906842624.0)
                {
                    unit = "tb";
                    dataset.Data[i] = Math.Round(dataset.Data[i] / 1099511627776.0, 1);
                }
            }

            if (Builder.Config.Plugins.Title != null &&
                !String.IsNullOrEmpty(Builder.Config.Plugins.Title.Text))
            {
                Builder.Config.Plugins.Title.Text += $" ({unit})";
            }

            return this;
        }
    }
}
