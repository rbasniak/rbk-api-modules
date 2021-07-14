using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class RadialChartBuilder<T> : BaseChartBuilder<RadialChartBuilder<T>, RadialChart>
    {
        internal IEnumerable<T> _originalData;

        public RadialChartBuilder(IEnumerable<T> data) : base(new RadialChart())
        {
            _originalData = data; 
        }

        public override bool HasData => Builder.Data.Datasets.SelectMany(x => x.Data).Any(x => x != 0);

        public static RadialChartBuilder<T> CreateRadialCategoryChart(IEnumerable<T> data)
        {
            return new RadialChartBuilder<T>(data);
        }

        public RadialChartBuilder<T> SetValuesRounding(int decimals)
        {
            var dataset = Builder.Data.Datasets.SingleOrDefault();

            if (dataset == null) throw new NotSupportedException($"Radial charts can have only one dataset");

            for (int i = 0; i < dataset.Data.Count; i++)
            {
                dataset.Data[i] = Math.Round(dataset.Data[i], decimals);
            }

            return this;
        }

        public RadialChartBuilder<T> Theme(params ColorPallete[] palletes)
        {
            return Colors(ChartCollorSelector.GetColors(palletes), "ff");
        }

        public RadialChartBuilder<T> Theme(string backgroundTransparency, params ColorPallete[] palletes)
        {
            return Colors(ChartCollorSelector.GetColors(palletes), backgroundTransparency);
        }

        public RadialChartBuilder<T> Colors(string[] colors, string backgroundTransparency = "ff")
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

        public RadialChartBuilder<T> RoundToNearestStorageUnit()
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

        public RadialDataBuilder<T> PreparaData()
        {
            return new RadialDataBuilder<T>(this);
        }
    }

    public class RadialDataBuilder<T>
    {
        private RadialChartBuilder<T> _radialChartBuilder;
        private Func<T, string> _seriesSelector;
        private Func<T, object> _converter;
        private bool _appendExtraData;
        private int? _decimalPlaces;
        private int _maximumSeries = Int32.MaxValue;
        private string _mergeLabel;
        private string _lastCall;
        private int? _topX;

        public RadialChartBuilder<T> Chart
        {
            get
            {
                if (_lastCall != nameof(ValueFrom)) throw new SafeException($"Last method called must be {nameof(ValueFrom)}");

                return _radialChartBuilder;
            }
        }

        public RadialDataBuilder(RadialChartBuilder<T> radialChartBuilder)
        {
            _radialChartBuilder = radialChartBuilder;
        }

        public RadialDataBuilder<T> AppendExtraData(Func<T, object> converter = null)
        {
            _appendExtraData = true;

            if (converter == null)
            {
                _converter = x => (object)x;
            }
            else
            {
                _converter = converter;
            }
            return this;
        }

        public RadialDataBuilder<T> SeriesFrom(Func<T, string> seriesSelector)
        {
            _seriesSelector = seriesSelector;

            return this;
        }

        public RadialDataBuilder<T> MaximumSeries(int value, string label)
        {
            _mergeLabel = label;
            _maximumSeries = value;

            return this;
        }

        public RadialDataBuilder<T> Take(int value)
        {
            _topX = value;

            return this;
        }

        public RadialDataBuilder<T> ValueFrom(Func<IGrouping<string, T>, double> valueSelector)
        {
            _lastCall = nameof(ValueFrom);

            var chart = new RadialChart();

            var serie = new RadialDataset();
            var groupedData = _radialChartBuilder._originalData.GroupBy(_seriesSelector).OrderByDescending(valueSelector).ToList();

            var untouchedData = new List<IGrouping<string, T>>();
            var mergedData = new List<IGrouping<string, T>>();

            if (groupedData.Count == _maximumSeries)
            {
                _maximumSeries = Int32.MaxValue;
            }

            for (int i = 0; i < _maximumSeries - 1; i++)
            {
                if (i >= groupedData.Count) break;

                untouchedData.Add(groupedData[i]);
            }

            for (int i = _maximumSeries - 1; i < groupedData.Count; i++)
            {
                mergedData.Add(groupedData[i]);
            }

            foreach (var serieData in untouchedData)
            {
                serie.Data.Add(valueSelector(serieData));
                if (_appendExtraData)
                {
                    serie.Extra.Add(serieData.Select(x => _converter(x)).ToList());
                }
                chart.Data.Labels.Add(serieData.Key);
            }

            if (mergedData.Count > 0)
            {
                var allMergedData = mergedData.SelectMany(x => x).GroupBy(x => "default").First();
                serie.Data.Add(valueSelector(allMergedData));

                if (_appendExtraData)
                {
                    serie.Extra.Add(allMergedData.Select(x => x).Select(x => _converter(x)).ToList());
                }

                chart.Data.Labels.Add(_mergeLabel);
            }

            if (_topX != null)
            {
                serie.Data = serie.Data.Take(_topX.Value).ToList();
                serie.Extra = serie.Extra.Take(_topX.Value).ToList();
                chart.Data.Labels = chart.Data.Labels.Take(_topX.Value).ToList();
            }

            chart.Data.Datasets.Add(serie);

            _radialChartBuilder.Builder.Data = chart.Data;

            return this;
        }

        public RadialDataBuilder<T> RoundValues(int decimalPlaces)
        {
            _decimalPlaces = decimalPlaces;

            return this;
        }
    }
}
