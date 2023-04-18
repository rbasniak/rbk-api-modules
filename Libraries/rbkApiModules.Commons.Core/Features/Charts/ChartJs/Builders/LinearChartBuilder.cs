using rbkApiModules.Commons.Core;
using System.Dynamic;

namespace rbkApiModules.Commons.Charts.ChartJs
{
    public class LinearChartBuilder<T> : BaseChartBuilder<LinearChartBuilder<T>, LinearChart>
    {
        internal IEnumerable<T> _originalData;

        public LinearChartBuilder(IEnumerable<T> originalData) : base(new LinearChart())
        {
            _originalData = originalData;
        }

        public override bool HasData => Builder.Data.Datasets.SelectMany(x => x.Data).Any(x => x.Y != 0);

        public override LinearChartBuilder<T> OfType(ChartType type)
        {
            if (type == ChartType.StackedBar)
            {
                WithAxis("x").Stacked();
                WithAxis("y").Stacked();
            }

            return base.OfType(type);
        } 

        public static LinearChartBuilder<T> CreateLinearDateChart(IEnumerable<T> data)
        {
            return new LinearChartBuilder<T>(data);
        }

        public static LinearChartBuilder<T> CreateLinearCategoryChart(IEnumerable<T> data)
        {
            return new LinearChartBuilder<T>(data);
        }

        public LinearDatasetBuilder<LinearChartBuilder<T>, LinearChart> SetupDataset(string datasetId)
        {
            var dataset = Builder.Data.Datasets.FirstOrDefault(x => x.Id == datasetId);

            if (dataset == null && Builder.Data.Datasets.Count > 0) throw new ArgumentException($"Unknown dataset: {datasetId}");

            var orderedDatasets = Builder.Data.Datasets.Where(x => x.Order != null).ToList();

            if (orderedDatasets != null && orderedDatasets.Count > 0)
            {
                if (dataset != null)
                {
                    dataset.Order = orderedDatasets.Max(x => x.Order) + 1;
                }
            }   
            else
            {
                if (dataset != null)
                {
                    dataset.Order = 0;
                }
            }

            return new LinearDatasetBuilder<LinearChartBuilder<T>, LinearChart>(this, new[] { dataset });
        }

        public LinearDatasetBuilder<LinearChartBuilder<T>, LinearChart> SetupDatasets()
        {
            return new LinearDatasetBuilder<LinearChartBuilder<T>, LinearChart>(this, Builder.Data.Datasets.ToArray());
        }

        public LinearChartBuilder<T> Theme(params ColorPallete[] palletes)
        {
            return SetColors(ChartCollorSelector.GetColors(palletes), "ff");
        }

        public LinearChartBuilder<T> Theme(string backgroundTransparency, params ColorPallete[] palletes)
        {
            return SetColors(ChartCollorSelector.GetColors(palletes), backgroundTransparency);
        }

        public LinearChartBuilder<T> SetColors(string[] colors, string backgroundTransparency = "ff")
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

        public CartesianScaleBuilder<LinearChartBuilder<T>, LinearChart> WithXAxis(string axisId)
        {
            return WithAxis(axisId);
        }

        public CartesianScaleBuilder<LinearChartBuilder<T>, LinearChart> WithYAxis(string axisId)
        {
            return WithAxis(axisId);
        }

        private CartesianScaleBuilder<LinearChartBuilder<T>, LinearChart> WithAxis(string axisId)
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

            return new CartesianScaleBuilder<LinearChartBuilder<T>, LinearChart>(this, (CartesianScale)axis);
        }

        public LinearDateDataBuilder<T> PreparaData(GroupingType type)
        {
            return new LinearDateDataBuilder<T>(this, type);
        }

        public LinearCategoryDataBuilder<T> PreparaData()
        {
            return new LinearCategoryDataBuilder<T>(this);
        }

        public HistogramDataBuilder<T> PreparaData(int intervalCount)
        {
            return new HistogramDataBuilder<T>(this, intervalCount);
        }

        public LinearChartBuilder<T> ReorderCategories(Func<List<Point>, IEnumerable<Point>> expression)
        {
            var labels = new List<string>();
            var datasets = new List<LinearDataset>();

            foreach (var dataset in Builder.Data.Datasets)
            {
                var orderedCategories = expression(dataset.Data.Select(x => x).ToList()).ToList();

                var dataPoints = new List<Point>();

                for (int i = 0; i < orderedCategories.Count; i++)
                {
                    var point = dataset.Data.FirstOrDefault(x => x.X == orderedCategories[i].X);

                    if (point != null)
                    {
                        var index = dataset.Data.IndexOf(point);
                        dataPoints.Add(dataset.Data[index]);
                    }
                }

                dataset.Data = dataPoints;
            }

            return this;
        }

        public LinearChartBuilder<T> ReorderCategories(params string[] orderedCategories)
        {
            var labels = new List<string>();
            var datasets = new List<LinearDataset>();

            foreach (var dataset in Builder.Data.Datasets)
            {
                var dataPoints = new List<Point>();

                for (int i = 0; i < orderedCategories.Length; i++)
                {
                    var point = dataset.Data.FirstOrDefault(x => x.X == orderedCategories[i]);

                    if (point != null)
                    {
                        var index = dataset.Data.IndexOf(point);
                        dataPoints.Add(dataset.Data[index]);
                    }
                    else
                    {
                        dataPoints.Add(new Point(orderedCategories[i], 0));
                    }
                }

                dataset.Data = dataPoints;
            }

            return this;
        }

        public LinearChartBuilder<T> SetColors(ColorPallete pallete, params int[] indices)
        {
            var colors = ChartCollorSelector.GetColors(pallete);

            for (int i = 0; i < Builder.Data.Datasets.Count; i++)
            {
                var color = i < colors.Length ? colors[indices[i]] : "#777777";

                Builder.Data.Datasets[i].BackgroundColor = color.ToLower() + "ff";
                Builder.Data.Datasets[i].BorderColor = color.ToLower();

                Builder.Data.Datasets[i].PointBackgroundColor = color.ToLower();
                Builder.Data.Datasets[i].PointBorderColor = color.ToLower();
            }

            return this;
        } 
    }

    public class LinearDateDataBuilder<T>
    {
        private LinearChartBuilder<T> _linearChartBuilder;
        private DateTime? _forceStartDate;
        private DateTime? _forceEndDate;
        private GroupingType _groupingType;
        private Func<T, string> _seriesSelector;
        private Func<T, DateTime> _dateSelector;
        private Func<T, object> _converter;
        private bool _appendExtraData;
        private int? _decimalPlaces;
        private string _lastCall;
        private int? _topX;
        private LinearChart _chart;

        public LinearChartBuilder<T> Chart
        {
            get
            {
                if (_lastCall != nameof(ValueFrom) && _lastCall != nameof(AddSerie)) throw new SafeException($"Last method called must be {nameof(ValueFrom)} or {nameof(AddSerie)}");

                return _linearChartBuilder;
            }
        }

        public LinearDateDataBuilder(LinearChartBuilder<T> linearChartBuilder, GroupingType type)
        {
            _groupingType = type;
            _linearChartBuilder = linearChartBuilder;
        } 

        public LinearDateDataBuilder<T> EnforceStartDate(DateTime date)
        {
            _lastCall = nameof(EnforceStartDate);

            _forceStartDate = date;
            return this;
        }

        public LinearDateDataBuilder<T> EnforceEndDate(DateTime date)
        {
            _lastCall = nameof(EnforceEndDate);

            _forceEndDate = date;
            return this;
        }
        public LinearDateDataBuilder<T> Take(int value)
        {
            _lastCall = nameof(Take);

            _topX = value;

            return this;
        }

        public LinearDateDataBuilder<T> AppendExtraData(Func<T, object> converter = null)
        {
            _lastCall = nameof(AppendExtraData);

            _appendExtraData = true;
            if (converter == null)
            {
                _converter = x => (object)x;
            }
            else if (converter != null)
            {
                _converter = converter;
            }
            return this;
        }

        public LinearDateDataBuilder<T> SeriesFrom(Func<T, string> seriesSelector)
        {
            _lastCall = nameof(SeriesFrom);

            _seriesSelector = seriesSelector;

            return this;
        }

        public LinearDateDataBuilder<T> SingleSerie()
        {
            _lastCall = nameof(SingleSerie);

            _seriesSelector = x => "default";

            return this;
        }

        public LinearDateDataBuilder<T> DateFrom(Func<T, DateTime> dateSelector)
        {
            _lastCall = nameof(DateFrom);

            _dateSelector = dateSelector;

            return this;
        }

        public LinearDateDataBuilder<T> ValueFrom(Func<IGrouping<DateTime, T>, double> valueSelector)
        {
            if (_dateSelector == null) throw new SafeException($"{nameof(DateFrom)} must be called before {nameof(ValueFrom)} in charts with Date axis");

            _lastCall = nameof(ValueFrom);

            if (_linearChartBuilder._originalData.Count() == 0) return this;

            var fromDate = _forceStartDate.HasValue ? _forceStartDate.Value : _linearChartBuilder._originalData.Min(x => _dateSelector(x));
            var toDate = _forceEndDate.HasValue ? _forceEndDate.Value : _linearChartBuilder._originalData.Max(x => _dateSelector(x));

            var chart = new LinearChart();

            foreach (var serieData in _linearChartBuilder._originalData.GroupBy(x => _seriesSelector(x)))
            {
                var serie = new LinearDataset(_seriesSelector(serieData.First()));

                serie.Data = BuildLineChartAxis(fromDate, toDate, _groupingType);

                foreach (var groupedSerieData in serieData.GroupBy(x => _dateSelector(x).GetGroupDate(_groupingType)))
                {
                    var point = serie.Data.Single(x => x.X == new Point(_dateSelector(groupedSerieData.First()), 0, _groupingType, null).X);

                    var value = valueSelector(groupedSerieData);

                    if (_decimalPlaces != null)
                    {
                        value = Math.Round(value, _decimalPlaces.Value);
                    }

                    point.Y = value;

                    if (_appendExtraData)
                    {
                        point.Data = groupedSerieData.Select(_converter);
                    }
                }

                chart.Data.Datasets.Add(serie);
            }

            chart.Data.Datasets = chart.Data.Datasets.OrderByDescending(x => x.Data.Sum(x => x.Y)).ToList();

            if (_topX != null)
            {
                chart.Data.Datasets = chart.Data.Datasets.Take(_topX.Value).ToList();
            }

            _linearChartBuilder.Builder.Data = chart.Data;

            return this; 
        }

        public LinearDateDataBuilder<T> AddSerie(string name, Func<IGrouping<DateTime, T>, double> valueSelector)
        {
            _lastCall = nameof(AddSerie);

            if (_linearChartBuilder._originalData.Count() == 0) return this;

            var fromDate = _forceStartDate.HasValue ? _forceStartDate.Value : _linearChartBuilder._originalData.Min(x => _dateSelector(x));
            var toDate = _forceEndDate.HasValue ? _forceEndDate.Value : _linearChartBuilder._originalData.Max(x => _dateSelector(x));

            if (_chart == null)
            {
                _chart = new LinearChart();
            }

            var serie = new LinearDataset(name);

            serie.Data = BuildLineChartAxis(fromDate, toDate, _groupingType);

            var serieData = _linearChartBuilder._originalData.GroupBy(x => "name").First();

            foreach (var groupedSerieData in serieData.GroupBy(x => _dateSelector(x).GetGroupDate(_groupingType)))
            {
                var point = serie.Data.Single(x => x.X == new Point(_dateSelector(groupedSerieData.First()), 0, _groupingType, null).X);

                var value = valueSelector(groupedSerieData);

                if (_decimalPlaces != null)
                {
                    value = Math.Round(value, _decimalPlaces.Value);
                }

                point.Y = value;

                if (_appendExtraData)
                {
                    point.Data = groupedSerieData.Select(_converter);
                }
            }

            _chart.Data.Datasets.Add(serie);

            _chart.Data.Datasets = _chart.Data.Datasets.OrderByDescending(x => x.Data.Sum(x => x.Y)).ToList();

            if (_topX != null)
            {
                throw new SafeException("TopX is not allowed with 'AddSeries'");
            }

            _linearChartBuilder.Builder.Data = _chart.Data;

            return this;
        }

        public LinearDateDataBuilder<T> RoundValues(int decimalPlaces)
        {
            _lastCall = nameof(RoundValues);

            _decimalPlaces = decimalPlaces;

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
    }

    public class LinearCategoryDataBuilder<T>
    {
        private LinearChartBuilder<T> _linearChartBuilder;
        private Func<T, string> _categorySelector;
        private Func<T, string> _seriesSelector;
        private Func<T, object> _converter;
        private bool _appendExtraData;
        private int? _decimalPlaces;
        private string _lastCall;

        public LinearChartBuilder<T> Chart
        {
            get
            {
                if (_lastCall != nameof(ValueFrom)) throw new SafeException($"Last method called must be {nameof(ValueFrom)}");

                return _linearChartBuilder;
            }
        }

        public LinearCategoryDataBuilder(LinearChartBuilder<T> linearChartBuilder)
        {
            _linearChartBuilder = linearChartBuilder;
        }

        public LinearCategoryDataBuilder<T> AppendExtraData(Func<T, object> converter = null)
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

        public LinearCategoryDataBuilder<T> CategoryFrom(Func<T, string> categorySelector)
        {
            _categorySelector = categorySelector;

            return this;
        }

        public LinearCategoryDataBuilder<T> SeriesFrom(Func<T, string> seriesSelector)
        {
            _seriesSelector = seriesSelector;

            return this;
        }

        public LinearCategoryDataBuilder<T> ValueFrom(Func<IGrouping<string, T>, double> valueSelector)
        {
            if (_categorySelector == null) throw new SafeException($"{nameof(CategoryFrom)} must be called before {nameof(ValueFrom)} in charts with Category axis");

            _lastCall = nameof(ValueFrom);

            var chart = new LinearChart();

            foreach (var serieData in _linearChartBuilder._originalData.GroupBy(x => _seriesSelector(x)))
            {
                var serie = new LinearDataset(_seriesSelector(serieData.First()));

                foreach (var groupedSerieData in serieData.GroupBy(x => _categorySelector(x)))
                {
                    var value = valueSelector(groupedSerieData);

                    if (_decimalPlaces != null)
                    {
                        value = Math.Round(value, _decimalPlaces.Value);
                    }

                    List<object> extraData = null;

                    if (_appendExtraData)
                    {
                        extraData = groupedSerieData.Select(_converter).ToList();
                    }

                    serie.Data.Add(new Point(_categorySelector(groupedSerieData.First()), value, extraData));
                }

                chart.Data.Datasets.Add(serie);
            }

            chart.Data.Datasets = chart.Data.Datasets.OrderByDescending(x => x.Data.Sum(x => x.Y)).ToList();

            // TODO: Top X
            //if (_topX != null)
            //{
            //    chart.Data.Datasets = chart.Data.Datasets.Take(_topX.Value).ToList();
            //}

            _linearChartBuilder.Builder.Data = chart.Data;

            return this;
        }

        public LinearCategoryDataBuilder<T> SingleSerie()
        {
            _seriesSelector = x => "default";

            return this;
        }

        //public LinearCategoryDataBuilder<T> ValueFrom(Func<IGrouping<string, T>, double> valueSelector)
        //{
        //    var chart = new LinearChart();

        //    foreach (var serieData in _linearChartBuilder._originalData.GroupBy(x => "default"))
        //    {
        //        var serie = new LinearDataset("default");

        //        foreach (var groupedSerieData in serieData.GroupBy(x => _categorySelector(x)))
        //        {
        //            var value = valueSelector(groupedSerieData);

        //            if (_decimalPlaces != null)
        //            {
        //                value = Math.Round(value, _decimalPlaces.Value);
        //            }

        //            List<object> extraData = new List<object>();

        //            if (_appendExtraData)
        //            {
        //                extraData = groupedSerieData.Select(_converter).ToList();
        //            }

        //            serie.Data.Add(new Point(groupedSerieData.Key.ToString(), value, extraData));

        //        }

        //        chart.Data.Datasets.Add(serie);
        //    }

        //    _linearChartBuilder.Builder.Data = chart.Data;

        //    return this;
        //}

        public LinearCategoryDataBuilder<T> RoundValues(int decimalPlaces)
        {
            _decimalPlaces = decimalPlaces;

            return this;
        }
    }

    public class HistogramDataBuilder<T>
    {
        private LinearChartBuilder<T> _linearChartBuilder;
        private Func<T, object> _converter;
        private bool _appendExtraData;

        private int _intervals;
        private double? _intervalRound;
        private double? _intervalMin;
        private double? _intervalMax;

        public LinearChartBuilder<T> Chart
        {
            get
            {
                return _linearChartBuilder;
            }
        }

        public HistogramDataBuilder(LinearChartBuilder<T> linearChartBuilder, int intervals)
        {
            _intervals = intervals;
            _linearChartBuilder = linearChartBuilder;
        }

        public HistogramDataBuilder<T> AppendExtraData(Func<T, object> converter = null)
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

        public HistogramDataBuilder<T> SetDesiredIntervalFraction(double value)
        {
            _intervalRound = value;

            return this;
        }

        public HistogramDataBuilder<T> ValuesFrom(Func<T, double> seriesSelector)
        {
            var chart = new LinearChart();

            var data = _linearChartBuilder._originalData;

            if (data.Count() == 0) return this;

            var min = _intervalMin ?? data.Min(x => seriesSelector(x));
            var max = _intervalMax ?? data.Max(x => seriesSelector(x));

            var delta = (max - min) / _intervals;

            if (_intervalRound != null)
            {
                min = Math.Floor(min / _intervalRound.Value) * _intervalRound.Value;
                max = (Math.Floor(max / _intervalRound.Value) + 1) * _intervalRound.Value;

                var delta1 = (Math.Floor(delta / _intervalRound.Value)) * _intervalRound.Value;
                var delta2 = (Math.Floor(delta / _intervalRound.Value) + 1) * _intervalRound.Value;

                var count1 = (max - min) / delta1;
                var count2 = (max - min) / delta2;

                var test1 = Math.Abs(count1 - _intervals);
                var test2 = Math.Abs(count2 - _intervals);

                if (test1 < test2)
                {
                    delta = delta1;
                }
                else
                {
                    delta = delta2;
                }

                var value = min;

                var intervals = 0;

                while(value < max)
                {
                    intervals++;
                    value += delta;
                }

                _intervals = intervals;
            }

            var serie = new LinearDataset("default");

            for (int i = 0; i < _intervals; i++)
            {
                double a = min + i * delta;
                double b = a + delta;

                var relatedData = data.Where(x => seriesSelector(x) >= a && seriesSelector(x) < b).ToList();

                List<object> extraData = null;

                if (_appendExtraData)
                {
                    extraData = relatedData.Select(_converter).ToList();
                }

                serie.Data.Add(new Point($"{a}", relatedData.Count, extraData));
            }

            chart.Data.Datasets.Add(serie);

            chart.Data.Labels = chart.Data.Datasets.First().Data.Select(x => x.X.ToString()).ToList();
            
            _linearChartBuilder.Builder.Data = chart.Data;

            return this;
        } 
    }

    public enum DatasetBuildMode
    {
        Sum,
        Average,
        Count
    }
}
