using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LinearDatasetBuilder<TFactory, TChart> : DatasetBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        private LinearDataset _dataset;

        public LinearDatasetBuilder(BaseChartBuilder<TFactory, TChart> builder, LinearDataset dataset): base(builder)
        {
            _dataset = dataset;
        }


        public LinearDatasetBuilder<TFactory, TChart> OfType(DatasetType type)
        {
            _dataset.SetDatasetType(type);

            if (type == DatasetType.Line)
            {
                _dataset.Tension = 0;
                _dataset.Fill = false;
            }
            else if (type == DatasetType.Bar)
            {
                _dataset.Fill = true;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> ValuesRounding(int decimals)
        {
            foreach (var item in _dataset.Data)
            {
                item.RoundValue(decimals);
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Label(string label)
        {
            _dataset.Label = label;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Thickness(double thickness)
        {
            _dataset.BorderWidth = thickness;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> CustomAxis(string axisId)
        {
            _dataset.yAxisID = axisId;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> Color(string color, string transparency = "ff")
        {
            _dataset.BackgroundColor = color + transparency;
            _dataset.BorderColor = color;

            _dataset.PointBackgroundColor = color;
            _dataset.PointBorderColor = color;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> BarPercentage(double value)
        {
            _dataset.BarPercentage = value;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> PointStyle(PointStyle style)
        {
            _dataset.SetPointStyle(style);

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> PointRadius(double radius, double? hitRadius = null)
        {
            _dataset.PointRadius = radius;

            if (hitRadius != null)
            {
                _dataset.PointHitRadius = hitRadius.Value;
            }

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> RoundedBorders(double radius)
        {
            _dataset.BorderRadius = radius;
            _dataset.BorderSkipped = false;

            return this;
        }

        public LinearDatasetBuilder<TFactory, TChart> RoundToNearestStorageUnit(bool showUnitsInChartTitle = false)
        {
            var averageSize = _dataset.Data.Average(x => x.Y);
            var unit = String.Empty;

            foreach (var item in _dataset.Data)
            {
                if (averageSize < 1024.0)
                {
                    unit = "bytes";
                }
                else if (averageSize < 1048576.0)
                {
                    unit = "kb";
                    item.Y = Math.Round(item.Y / 1024.0, 1);
                }
                else if (averageSize < 1073741824.0)
                {
                    unit = "mb";
                    item.Y = Math.Round(item.Y / 1048576.0, 1);
                }
                else if (averageSize < 1099511627776.0)
                {
                    unit = "gb";
                    item.Y = Math.Round(item.Y / 1073741824.0, 1);
                }
                else if (averageSize < 1125899906842624.0)
                {
                    unit = "tb";
                    item.Y = Math.Round(item.Y / 1099511627776.0, 1);
                }

                if (!String.IsNullOrEmpty(_dataset.Label) && !_dataset.Label.EndsWith($" ({unit})")) _dataset.Label += $" ({unit})";
            }

            if (Builder.Builder.Config.Plugins.Title != null && 
                !String.IsNullOrEmpty(Builder.Builder.Config.Plugins.Title.Text) &&
                showUnitsInChartTitle)
            {
                Builder.Builder.Config.Plugins.Title.Text += $" ({unit})";
            }

            return this;
        }
    } 
}